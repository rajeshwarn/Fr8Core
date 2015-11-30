﻿/// <reference path="../../_all.ts" />

module dockyard.directives {
    import pwd = dockyard.directives.paneWorkflowDesigner;
    'use strict';

    export function RunRouteButton ($compile: ng.ICompileService): ng.IDirective {
        var runContainer = function ($q, $http, routeId): ng.IPromise<any> {
            var url = '/api/routes/run?routeId=' + routeId;

            return $q(function (resolve, reject) {
                $http.post(url)
                    .then(function (res) {
                        resolve(res.data);
                    })
                    .catch(function (err) {
                        reject(err);
                    });
            });
        };

        var getRoute = function ($q, $http, actionId): ng.IPromise<any> {
            var url = '/api/routes/getByAction/' + actionId;

            return $q(function (resolve, reject) {
                $http.get(url)
                    .then(function (res) {
                        resolve(res.data);
                    })
                    .catch(function (err) {
                        reject(err);
                    });
            });
        };
        
        return {
            restrict: 'E',
            templateUrl: '/AngularTemplate/RunRouteButton',
            scope: {
                currentAction: '=',
            },
            controller: ['$scope', '$http', '$q', '$location', 
                function (
                    $scope: IRunRouteButtonScope,
                    $http: ng.IHttpService,
                    $q: ng.IQService,
                    $location: ng.ILocationService
                ) {

                    $scope.runNow = function () {
                        $scope.error = null;

                        $scope.$emit(pwd.MessageType[pwd.MessageType.PaneWorkflowDesigner_LongRunningOperation], new pwd.LongRunningOperationEventArgs(pwd.LongRunningOperationFlag.Started));

                        getRoute($q, $http, $scope.currentAction.id)
                            .then(function (route) {
                                runContainer($q, $http, route.id)
                                    .then(function (container) {
                                        var path = '/findObjects/' + container.id + '/results';
                                        $location.path(path);
                                    })
                                    .catch(function (err) {
                                        $scope.error = err;
                                    })
                                    .finally(function () {
                                        $scope.$emit(pwd.MessageType[pwd.MessageType.PaneWorkflowDesigner_LongRunningOperation], new pwd.LongRunningOperationEventArgs(pwd.LongRunningOperationFlag.Stopped));
                                    });
                            })
                            .catch(function (err) {
                                $scope.error = err;
                                $scope.$emit(pwd.MessageType[pwd.MessageType.PaneWorkflowDesigner_LongRunningOperation], new pwd.LongRunningOperationEventArgs(pwd.LongRunningOperationFlag.Stopped));
                            });
                    };
                }
            ]
        }
    }

    export interface IRunRouteButtonScope extends ng.IScope {
        currentAction: model.ActionDTO;
        error: string;
        runNow: () => void;
    }
}

app.directive('runRouteButton', dockyard.directives.RunRouteButton); 