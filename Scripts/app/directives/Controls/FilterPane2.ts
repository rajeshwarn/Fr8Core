﻿/// <reference path="../../_all.ts" />

module dockyard.directives {
    'use strict';

    export function FilterPane2(): ng.IDirective {
        var uniqueDirectiveId = 1;
        return {
            restrict: 'E',
            templateUrl: '/AngularTemplate/FilterPane2',
            scope: {
                currentAction: '=',
                field: '=',
                change: '&'
            },
            controller: ['$scope', '$timeout', 'CrateHelper',
                function (
                    $scope: IPaneDefineCriteria2Scope,
                    $timeout: ng.ITimeoutService,
                    crateHelper: services.CrateHelper
                ) {
                    $scope.uniqueDirectiveId = ++uniqueDirectiveId;

                    $scope.conditions = [];

                    $scope.$watch('field', function (newValue: any) {
                        if (newValue && newValue.value) {
                            var jsonValue = angular.fromJson(newValue.value);

                            $scope.conditions = jsonValue.conditions;
                            if (!$scope.conditions) {
                                $scope.conditions = [];
                            }

                            $scope.executionType = jsonValue.executionType;
                        }
                        else {
                            $scope.conditions = [
                                {
                                    field: null,
                                    operator: '',
                                    value: null
                                }
                            ];
                            $scope.executionType = 2;
                        }
                    });

                    var updateFieldValue = function () {
                        $scope.field.value = angular.toJson({
                            executionType: $scope.executionType,
                            conditions: $scope.conditions
                        });
                    };

                    $scope.$watch('conditions', () => { 
                        updateFieldValue();
                    }, true);

                    $scope.$watch('executionType', (oldValue, newValue) => {
                        updateFieldValue();

                        if (oldValue === newValue) {
                            return;
                        }

                        // Invoke onChange event handler
                        if ($scope.change != null && angular.isFunction($scope.change)) {
                            $scope.change()($scope.field);
                        }
                    });
                }
            ]
        }
    }

    export interface IPaneDefineCriteria2Scope extends ng.IScope {
        field: any;
        change: () => (field: model.ControlDefinitionDTO) => void;
        fields: Array<IQueryField2>;
        conditions: Array<IQueryCondition2>;
        executionType: number;
        uniqueDirectiveId: number;
    }
}

app.directive('filterPaneTwo', dockyard.directives.FilterPane2);