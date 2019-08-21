// Define the `checkEnvApp` module
var checkEnvApp = angular.module('CheckEnvApp', ["chart.js"]);
// CheckEnvController.$inject = [$scope, $http, $timeout, $interval];
CheckEnvController.$inject = ['$scope', '$http', '$timeout', '$interval'];
checkEnvApp.controller('CheckEnvController', CheckEnvController);

//var CheckEnvController =
function CheckEnvController($scope, $http, $timeout, $interval) {
    $scope.environments = [
      {
          name: 'DEV'
      }, {
          name: 'PP'
      }, {
          name: 'Prod'
      }
    ];
    $scope.topicsubcount = [
        {
            Name: 'Validation',
            Count: 0,
            timestamp: '',
            lastsuccesstimestamp: ''
        },
        {
            Name: 'Funding',
            Count: 0,
            timestamp: '',
            lastsuccesstimestamp: ''
        },
        {
            Name: 'Deds',
            Count: 0,
            timestamp: '',
            lastsuccesstimestamp: ''
        },
        {
            Name: 'Reports',
            Count: 0,
            timestamp: '',
            lastsuccesstimestamp: ''
        }
    ];

    
    var displayTimeString = function () {
        var d = new Date();
        return d.toLocaleTimeString();
    };

    // the chart labels, everytime we get a data point we add a label with the current timestamp
    $scope.labels = [displayTimeString()];
    $scope.labelsNoTime = ['Now'];
    $scope.jobLabels = [displayTimeString()];
    // the chart then has several set of data (series), one data point per x-axis "label"
    $scope.series = ['Deds'];
    $scope.jobSeries = ['submitted','processing','failed','completed'];

    $scope.data = [
      [0],
      [0],
      [0],
      [0],
        [0],
        [0],
    ];
    $scope.dataNoTime = [
        [0],
        [0],
        [0],
        [0],
        [0],
        [0]        
    ];
    // array of job counts (probably by status in teh last five minutes)
    $scope.jobData = [
        [0],
        [0],
        [0],
        [0],
    ];
    $scope.red = '#FDB45C';
    $scope.green = '#B4FD5C';
    $scope.maxbars = 60;
    $scope.maxjobbars = 30;
    $scope.timestarted = new Date();
    $scope.started = false;
    $scope.startedJobs = false;

    $scope.colors = ['#FDB45C', '#B4FD5C', '#B45CFD', '#B45C5C', '#FDB45C', '#FDB45C', '#FDB45C'];
    //  [200],
    //  [408],
    //  [408],
    //  [408],
    //  [408]
    //];
             
    // keeps a running tally of time until next invocation in milli-seconds
    $scope.time = 0;
    $scope.promise = null;
    $scope.promiseJob = null;
    //timer callback
    var timer = function () {
        $scope.time += 1000;
        $timeout(timer, 1000);
    }

    $scope.doacheck = function (e) {
        $scope.message = "Working..." + e;
        $scope.started = true;
        if ($scope.promise == null) {
            $scope.promise = $interval(function () { $scope.doacheck(e); }, 1000); // 120000
            $scope.timestarted = new Date();
        }
        $http(
                    {
                        method: 'POST',
                        url: '/queue/GetTopicData',
                        data: { id: e }
                    }).then(function (rc) {
                        $scope.message = "Completed series count " + $scope.series.length;

                        $scope.labels.push(displayTimeString());
                        // if we've added too much data then simply drop the oldest
                        if ($scope.labels.length > $scope.maxbars)
                        {
                            $scope.labels.shift();
                        }
                        // data coming back is { "name": "", activemessagecount: ""}
                        $scope.message += " got " + rc.data.length;
                        for (var i = 0; i != rc.data.length; ++i)
                        {
                            if (i >= $scope.series.length) {
                                $scope.message += "Added " + rc.data[i].Name;
                                $scope.series.push(rc.data[i].Name);
                            }

//                            $scope.message += "(0)";
                            $scope.series[i] = rc.data[i].Name;
                            $scope.data[i].push(rc.data[i].ActiveMessageCount);
                            $scope.dataNoTime[i][0] = rc.data[i].ActiveMessageCount;
//                            $scope.message += "(1)";

                            if ($scope.data[i].length > $scope.maxbars)
                            {
                                $scope.data[i].shift();
                            }
//                            $scope.message += "(2)";
//                            $scope.colors[i] = worked ? $scope.green : $scope.red;
                            var matched = false;
                            for (var j = 0; j != $scope.topicsubcount.length; ++j)
                            {
                                //$scope.series.push($scope.webstatus[j].name);
                                if ($scope.topicsubcount[j].Name == rc.data[i].Name)
                                {
                                    $scope.topicsubcount[j].Count = rc.data[i].ActiveMessageCount;
                                    $scope.topicsubcount[j].timestamp = new Date();
                                    matched = true;
                                    break;
                                }
//                                $scope.message += "(3)";
                            }
                            if( !matched)
                            {
//                                $scope.message += "(4)";
                                $scope.topicsubcount.push({ Name: rc.data[i].Name, Count: rc.data[i].ActiveMessageCount, timestamp: new Date() });
                            }
                        }
//                        $scope.message += "(5)";
                        $scope.time = 0;
                        $scope.message += "finished:";
                    }
);;
    };
    $scope.joblook = function (e) {
        $scope.message = "Working..." + e;
        $scope.startedJobs = true;
        if ($scope.promiseJob == null) {
            $scope.promiseJob = $interval(function () { $scope.joblook(4); }, 120000); // 120000
            $scope.timestarted = new Date();
        }
        $http(
            {
                method: 'POST',
                url: '/queue/GetJobCount',
                data: { time: new Date() }
            }).then(function (rc) {
//                $scope.message = "Completed GetJobCount " + $scope.jobSeries.length;

                $scope.jobLabels.push(displayTimeString());
                // if we've added too much data then simply drop the oldest
                if ($scope.jobLabels.length > $scope.maxjobbars) {
                    $scope.jobLabels.shift();
                }
                // data coming back is { ["0","1","2"]}
//                $scope.message += " got " + rc.data.length;
                for (var i = 0; i != rc.data.length; ++i) {
                    //if (i >= $scope.series.length) {
                    //    $scope.message += "Added " + rc.data[i].Name;
                    //    $scope.series.push(rc.data[i].Name);
                    //}

//                    $scope.message += "(0)";
                    $scope.message += rc.data[i];
                    $scope.jobData[i].push(rc.data[i]);
//                                                $scope.message += "(1)";

                    if ($scope.jobData[i].length > $scope.maxjobbars) {
                        $scope.jobData[i].shift();
                    }
  //                                              $scope.message += "(2)";
                    //                            $scope.colors[i] = worked ? $scope.green : $scope.red;
                }
//                                        $scope.message += "(5)";
//                $scope.time = 0;
//                $scope.message += "finished:";
            }
            );;
    };
};



