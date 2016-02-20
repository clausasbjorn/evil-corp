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
        $scope.locationMinMax = [
            { min: 0, max: -100, range: 100 },
            { min: 0, max: -100, range: 100 },
            { min: 0, max: -100, range: 100 }
        ];
        $scope.coords = {};
        $scope.selectedId = null;

        var triangulate = function(scope, id, locations) {
            if (locations.length !== 3)
                return;

            var x = (locations[0].SignalStrength - scope.locationMinMax[0].max);
            var y = (locations[1].SignalStrength - scope.locationMinMax[1].max);
            var z = (locations[2].SignalStrength - scope.locationMinMax[2].max);

            if (x > 0 || y > 0 || z > 0) return;

            var x1 = (Math.abs(x) / scope.locationMinMax[0].range) * 100;
            var y1 = (Math.abs(y) / scope.locationMinMax[1].range) * 100;
            var z1 = (Math.abs(z) / scope.locationMinMax[2].range) * 100;

            if (x1 > 100) x1 = 100; if (x1 < 0) x1 = 0;
            if (y1 > 100) y1 = 100; if (y1 < 0) y1 = 0;
            if (z1 > 100) z1 = 100; if (z1 < 0) z1 = 0;

            var width = 500;
            var diagonal = 707;

            var diagonalX = diagonal * (x1 / 100);
            var diagonalY = diagonal * (y1 / 100);
            var diagonalZ = diagonal * (z1 / 100);

            //console.log(diagonalX + " " + diagonalY + " " + diagonalZ);

            var coordX = Math.sqrt(Math.pow((diagonalX / 2), 2) * 2);
            var coordY = Math.sqrt(Math.pow((diagonalY / 2), 2) * 2);
            var coordZ = Math.sqrt(Math.pow((diagonalZ / 2), 2) * 2);

            if (!scope.coords[id])
                scope.coords[id] = [];

            scope.coords[id].splice(0, 0, { c0: coordX, c1: coordY, c2: coordZ });
            //scope.coords[id] = scope.coords[id].slice(0, 10);
        }

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
                var timestamp = location.LastSeen;
                var index = scope.locationIndex[hotspot];
                var signalStrength = location.SignalStrength;

                var newLastMessage = new Date();
                if (scope.locations[index].users[id]) {
                    var currentTimestamp = scope.locations[index].users[id].timestamp;
                    if (currentTimestamp === timestamp) {
                        newLastMessage = scope.locations[index].users[id].lastMessage;
                    }
                }

                if (scope.locationMinMax[index].min > signalStrength) {
                    scope.locationMinMax[index].min = signalStrength;
                    scope.locationMinMax[index].range = Math.abs(scope.locationMinMax[index].min - scope.locationMinMax[index].max);
                }

                if (scope.locationMinMax[index].max < signalStrength) {
                    scope.locationMinMax[index].max = signalStrength;
                    scope.locationMinMax[index].range = Math.abs(scope.locationMinMax[index].min - scope.locationMinMax[index].max);
                }

                triangulate(scope, id, o.Locations);

                scope.locations[index].users[id] = {
                    id: id,
                    timestamp: timestamp,
                    lastMessage: newLastMessage,
                    signalStrength: signalStrength
                }
            }
        }

        var canvas = oCanvas.create({ canvas: "#trackermap" });
        var points = [];
        var lines = [];

        var reset = function() {
            for (var i = 0; i < points.length; i++)
                canvas.removeChild(points[i]);

            for (var j = 0; j < lines.length; j++)
                canvas.removeChild(lines[j]);
        }

        var drawLine = function (x1, y1, x2, y2) {
            var line = canvas.display.line({
                start: { x: x1, y: y1 },
                end: { x: x2, y: y2 },
                stroke: "2px #fff"
            });

            lines.push(line);
            canvas.addChild(line);
        }

        var drawPoint = function (x, y) {
            var point = canvas.display.ellipse({
                x: x,
                y: y,
                radius: 5,
                stroke: "transparent",
                fill: "#fff"
            });
            canvas.addChild(point);
            points.push(point);
        }

        
        var updateDrawing = function(scope) {
            var id = scope.selectedId;
            if (!id) return;
            if (!scope.coords[id]) return;

            var coords = scope.coords[id][0];

            var c0 = coords.c0;

            var c1X = 500 - coords.c1;
            var c1Y = coords.c1;

            var c2X = coords.c2;
            var c2Y = 500 - coords.c2;

            reset();
            drawPoint(c0, c0);
            drawPoint(c1X, c1Y);
            drawPoint(c2X, c2Y);

            drawLine(c0, c0, c2X, c2Y);
            drawLine(c1X, c1Y, c0, c0);
            drawLine(c2X, c2Y, c1X, c1Y);

            console.log(coords);
        }

        var washLocation = function (scope, index, location) {
            var c = 0;
            for (var i in location.users) {
                var user = location.users[i];
                var since = ((new Date()) - user.lastMessage);
                if (since > 90000)
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

        $scope.strengthRank = function(index, measurement) {
            var mm = $scope.locationMinMax[index];
            var section = mm.range / 3;
            if (measurement < (mm.min + section))
                return 0;
            if (measurement < (mm.min + (section * 2)))
                return 1;
            return 2;
        }

        $scope.select = function(id) {
            $scope.selectedId = id;
        }

        $scope.friendlyName = function(name) {
            if (name === 'b8:27:eb:31:86:7e') return "PI-1 - The Room With a View";
            if (name === 'b8:27:eb:fc:80:ea') return "PI-2 - Evil Lair";
            if (name === 'b8:27:eb:ff:d2:b0') return "PI-3 - Kitchen";
        };

        $scope.fromNow = function(date) {
            return moment(date).fromNow() + " " + Math.random();
        };

        $scope.seen = function(scope, data) {
            update(scope, parse(data));
            updateDrawing(scope);
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
