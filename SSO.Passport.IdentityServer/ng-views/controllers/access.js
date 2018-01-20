myApp.controller('access', ["$timeout", "$state","NgTableParams", "$scope", "$http", function ($timeout, $state,NgTableParams, $scope, $http) {
	window.hub.disconnect();
	$scope.loading();
	var self = this;
	self.stats = [];
	self.data = {};
	$scope.kw = "";
	$scope.orderby = 1;
	$scope.paginationConf = {
		currentPage:  1,
		//totalItems: $scope.total,
		itemsPerPage: 10,
		pagesLength: 25,
		perPageOptions: [1, 5, 10, 15, 20, 30, 40, 50, 100, 200],
		rememberPerPage: 'perPageItems',
		onChange: function() {
			self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
		}
	};
}]);