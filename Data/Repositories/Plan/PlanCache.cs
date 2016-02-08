using System;
using System.Collections.Generic;
using System.Linq;
using Data.Entities;
using Data.States;

namespace Data.Repositories.Plan
{
    public class PlanCache : IPlanCache
    {
        /**********************************************************************************/

        private class CachedRoute
        {
            public RouteNodeDO Root { get; private set; }
            public IExpirationToken Expiration { get; set; }

            public CachedRoute(RouteNodeDO root, IExpirationToken expiration)
            {
                Root = root;
                Expiration = expiration;
            }
        }

        /**********************************************************************************/

        private class CacheItem
        {
            public readonly RouteNodeDO Node;
            public readonly CachedRoute Route;

            public CacheItem(RouteNodeDO node, CachedRoute route)
            {
                Node = node;
                Route = route;
            }
        }

        /**********************************************************************************/
        // Declarations
        /**********************************************************************************/
        
        private readonly Dictionary<Guid, CacheItem> _routeNodesLookup = new Dictionary<Guid, CacheItem>();
        private readonly Dictionary<Guid, CachedRoute> _routes = new Dictionary<Guid, CachedRoute>();
        private readonly object _sync = new object();
        private readonly IPlanCacheExpirationStrategy _expirationStrategy;
        
        /**********************************************************************************/
        // Functions
        /**********************************************************************************/

        public PlanCache(IPlanCacheExpirationStrategy expirationStrategy)
        {
            _expirationStrategy = expirationStrategy;
            expirationStrategy.SetExpirationCallback(RemoveExpiredRoutes);
        }
        
        /**********************************************************************************/

        public RouteNodeDO Get(Guid id, Func<Guid, RouteNodeDO> cacheMissCallback)
        {
            RouteNodeDO node;

            lock (_sync)
            {
                CacheItem cacheItem;

                if (!_routeNodesLookup.TryGetValue(id, out cacheItem))
                {
                    node = cacheMissCallback(id);

                    if (node == null)
                    {
                        return null;
                    }

                    // Get the root of RouteNode tree. 
                    while (node.ParentRouteNode != null)
                    {
                        node = node.ParentRouteNode;
                    }

                    // Check cache integrity
                    if (RouteTreeHelper.Linearize(node).Any(x => _routeNodesLookup.ContainsKey(x.Id)))
                    {
                        DropCachedRoute(node);
                    }

                    AddToCache(node);
                }
                else
                {
                    node = cacheItem.Route.Root;
                    // update route expiration
                    cacheItem.Route.Expiration = _expirationStrategy.NewExpirationToken();
                }
            }

            return node;
        }

        /**********************************************************************************/
        
        public void UpdateElements(Action<RouteNodeDO> updater)
        {
            lock (_sync)
            {
                foreach (var cacheItem in _routeNodesLookup.Values)
                {
                    updater(cacheItem.Node);
                }
            }
        }

        /**********************************************************************************/

        public void UpdateElement(Guid id, Action<RouteNodeDO> updater)
        {
            lock (_sync)
            {
                CacheItem node;

                if (_routeNodesLookup.TryGetValue(id, out node))
                {
                    updater(node.Node);
                }
            }
        }

        /**********************************************************************************/

        public void Update(RouteNodeDO node)
        {
            // Get the root of RouteNode tree. 
            while (node.ParentRouteNode != null)
            {
                node = node.ParentRouteNode;
            }

            lock (_sync)
            {
                DropCachedRoute(node);
                AddToCache(node);
            }
        }

        /**********************************************************************************/

        private void AddToCache(RouteNodeDO root)
        {
            var expirOn = _expirationStrategy.NewExpirationToken();
            var cachedRoute = new CachedRoute(root, expirOn);
            _routes.Add(root.Id, cachedRoute);

            RouteTreeHelper.Visit(root, x => _routeNodesLookup.Add(x.Id, new CacheItem(x, cachedRoute)));
        }

        /**********************************************************************************/

        private void DropCachedRoute(RouteNodeDO root)
        {
            CachedRoute cachedRoute;
            
            if (!_routes.TryGetValue(root.Id, out cachedRoute))
            {
                return;
            }

            RouteTreeHelper.Visit(root, x => _routeNodesLookup.Remove(x.Id));
            
            _routes.Remove(root.Id);
        }

        /**********************************************************************************/

        private void RemoveExpiredRoutes()
        {
            lock (_sync)
            {
                foreach (var routeExpiration in _routes.ToArray())
                {
                    if (routeExpiration.Value.Expiration.IsExpired())
                    {
                        _routes.Remove(routeExpiration.Key);
                        RouteTreeHelper.Visit(routeExpiration.Value.Root, x => _routeNodesLookup.Remove(x.Id));
                    }
                }
            }
        }

        /**********************************************************************************/
    }
}