﻿module dockyard.controllers {

    export interface IAuthenticationDialogScope extends ng.IScope {
        actionIds: Array<number>;
        terminals: Array<model.ManageAuthToken_TerminalDTO>;

        isLoading: () => boolean;
        isAllSelected: () => boolean;
        linkAccount: (terminal: model.ManageAuthToken_TerminalDTO) => void;
        apply: () => void;
        $close: () => void;
    }


    export class AuthenticationDialogController {
        public static $inject = [
            '$scope',
            '$http',
            '$window',
            '$modal',
            'urlPrefix'
        ];

        constructor(
            private $scope: IAuthenticationDialogScope,
            private $http: ng.IHttpService,
            private $window: ng.IWindowService,
            private $modal: any,
            private urlPrefix: string
            ) {

            var _terminalActions = [];
            var _loading = false;

            $scope.terminals = [];

            $scope.linkAccount = function (terminal) {
                if (terminal.authenticationType === 2 || terminal.authenticationType === 4) {
                    _authenticateInternal(terminal);
                }
                else if (terminal.authenticationType === 3) {
                    _authenticateExternal(terminal);
                }
            };

            $scope.apply = function () {
                if (!$scope.isAllSelected()) {
                    return;
                }

                var data = [];

                var i, j;
                var terminalId;
                for (i = 0; i < _terminalActions.length; ++i) {
                    terminalId = _terminalActions[i].terminal.id;
                    for (j = 0; j < $scope.terminals.length; ++j) {
                        if ($scope.terminals[j].id === terminalId) {
                            data.push({
                                actionId: _terminalActions[i].actionId,
                                authTokenId: (<any>$scope.terminals[j]).selectedAuthTokenId,
                                isMain: (<any>$scope.terminals[j]).isMain
                            });
                            break;
                        }
                    }
                }

                _loading = true;

                $http.post(urlPrefix + '/ManageAuthToken/apply', data)
                    .then(function (res) {
                        $scope.$close();
                    })
                    .finally(function () {
                        _loading = false;
                    });
            };

            $scope.isLoading = function () {
                return _loading;
            };

            $scope.isAllSelected = function () {
                var i;
                for (i = 0; i < $scope.terminals.length; ++i) {
                    if (!(<any>$scope.terminals[i]).selectedAuthTokenId) {
                        return false;
                    }
                }

                return true;
            };

            var _authenticateInternal = function (terminal: model.ManageAuthToken_TerminalDTO) {
                var modalScope = <any>$scope.$new(true);
                modalScope.terminalId = terminal.id;
                modalScope.mode = terminal.authenticationType;

                $modal.open({
                    animation: true,
                    templateUrl: '/AngularTemplate/InternalAuthentication',
                    controller: 'InternalAuthenticationController',
                    scope: modalScope
                })
                .result
                .then(() => _reloadTerminals());
            };

            var _authenticateExternal = function (terminal: model.ManageAuthToken_TerminalDTO) {
                var self = this;
                var childWindow;
                
                var messageListener = function (event) {
                    if (!event.data || event.data != 'external-auth-success') {
                        return;
                    }
                
                    childWindow.close();
                    _reloadTerminals();
                };
                
                $http
                    .get('/api/authentication/initial_url?id=' + terminal.id)
                    .then(res => {
                        var url = (<any>res.data).url;
                        childWindow = $window.open(url, 'AuthWindow', 'width=400, height=500, location=no, status=no');
                        window.addEventListener('message', messageListener);
                
                        var isClosedHandler = function () {
                            if (childWindow.closed) {
                                window.removeEventListener('message', messageListener);
                            }
                            else {
                                setTimeout(isClosedHandler, 500);
                            }
                        };
                        setTimeout(isClosedHandler, 500);
                    });
            };

            var _reloadTerminals = function () {
                var actionIds = $scope.actionIds || [];

                _loading = true;

                $http.post(
                    urlPrefix + '/ManageAuthToken/TerminalsByActions',
                    actionIds
                )
                .then(function (res) {
                    var terminals: Array<model.ManageAuthToken_TerminalDTO> = [];
                    _terminalActions = <any>res.data;

                    var i, j, wasAdded;
                    for (i = 0; i < _terminalActions.length; ++i) {
                        wasAdded = false;

                        for (j = 0; j < terminals.length; ++j) {
                            if (terminals[j].id === _terminalActions[i].terminal.id) {
                                wasAdded = true;
                                break;
                            }
                        }

                        if (!wasAdded) {
                            terminals.push(<model.ManageAuthToken_TerminalDTO>_terminalActions[i].terminal);
                        }
                    }

                    $scope.terminals = terminals;
                })
                .finally(function () {
                    _loading = false;
                });
            };

            _reloadTerminals();
        }
    }

    app.controller('AuthenticationDialogController', AuthenticationDialogController);
} 