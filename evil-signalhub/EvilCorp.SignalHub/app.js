(function () {

    var app = angular.module('evilApp', ['SignalR']).factory('Tracking', ['$rootScope', 'Hub', '$timeout', function ($rootScope, Hub, $timeout) {

        var callbacks = [];

        //declaring the hub connection
        var hub = new Hub('TrackerHub', {

            //client side methods
            listeners: {
                'seen': function (data) {
                    for (var i = 0; i < callbacks.length; i++) {
                        callbacks[i](data);
                    }
                }
            },

            //server side methods
            methods: [],

            //query params sent on initial connection
            queryParams: { },

            //handle connection error
            errorHandler: function (error) {
                console.error(error);
            },

            //specify a non default root
            //rootPath: '/api

            stateChanged: function (state) {
                switch (state.newState) {
                    case $.signalR.connectionState.connecting:
                        //your code here
                        break;
                    case $.signalR.connectionState.connected:
                        //your code here
                        break;
                    case $.signalR.connectionState.reconnecting:
                        //your code here
                        break;
                    case $.signalR.connectionState.disconnected:
                        //your code here
                        break;
                }
            }
        });

        return {
            registerOnSeen: function(callback) {
                callbacks.push(callback);
            }
        };
    }]);

    app.controller('DashboardController', ['$scope', 'Tracking', '$timeout', function ($scope, Tracking) {

        $scope.title = "EvilCorp HQ - Dashboard";
        $scope.users = { };
        $scope.locations = [{
            id: 'b8:27:eb:31:86:7e',
            users: {}
        }, {
            id: 'b8:27:eb:fc:80:ea',
            users: {}
        }, {
            id: 'b8:27:eb:ff:d2:b0',
            users: {}
        }];

        $scope.locationCount = 2;
        $scope.locationIndex = {
            'b8:27:eb:31:86:7e': 0,
            'b8:27:eb:fc:80:ea': 1,
            'b8:27:eb:ff:d2:b0': 2
        };
        $scope.locationPeopleCount = [0, 0, 0];

        var parse = function(str) {
            return JSON.parse(str);
        }

        var update = function(scope, o) {
            var id = o.Id;
            scope.users[id] = {
                id: id,
                lastMessage: new Date(),
                locations: o.Locations
            };

            for (var i = 0; i < o.Locations.length; i++) {
                var location = o.Locations[i];
                var hotspot = location.Hotspot;
                var index = scope.locationIndex[hotspot];
                var signalStrength = location.SignalStrength;

                scope.locations[index].users[id] = {
                    id: id,
                    lastMessage: new Date(),
                    signalStrength: signalStrength
                }
            }
        }

        var washLocation = function (scope, index, location) {
            var c = 0;
            for (var i in location.users) {
                var user = location.users[i];
                var since = ((new Date()) - user.lastMessage);
                if (since > 30000)
                    delete location.users[i];
                else
                    c++;
            }
            scope.locationPeopleCount[index] = c;

            return location;
        }

        var wash = function (scope) {
            var locations = scope.locations;
            for (var i = 0; i < locations.length; i++)
                scope.locations[i] = washLocation(scope, i, locations[i]);
        }

        $scope.friendlyName = function (name) {
            if (name === 'b8:27:eb:31:86:7e') return "PI-1";
            if (name === 'b8:27:eb:fc:80:ea') return "PI-2";
            if (name === 'b8:27:eb:ff:d2:b0') return "PI-3";
        }

        $scope.fromNow = function(date) {
            return moment(date).fromNow() + " " + Math.random();
        };

        $scope.seen = function(scope, data) {
            update(scope, parse(data));
            wash(scope);
        };

        (function(scope) {
            Tracking.registerOnSeen(function (data) {
                scope.seen(scope, data);
                scope.$apply();
            });
        })($scope);

    }]);

})();
