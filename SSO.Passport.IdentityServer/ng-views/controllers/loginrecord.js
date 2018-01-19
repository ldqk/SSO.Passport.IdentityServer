myApp.controller("loginrecord", ["$scope", "$http", "NgTableParams", function($scope, $http, NgTableParams) {
	window.hub.disconnect();
	var self = this;
	$scope.loading();
	$http.post("/passport/loginrecord/").then(function(res) {
		self.tableParams = new NgTableParams({
			count: 15
		}, {
			filterDelay: 0,
			dataset: res.data.Data.list
		});
		$scope.loadingDone();
	});
}]);