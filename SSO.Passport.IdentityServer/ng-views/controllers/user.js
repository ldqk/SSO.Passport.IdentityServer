myApp.controller('user', ["$timeout", "$state", "NgTableParams", "$scope", "$http",function($timeout, $state, NgTableParams, $scope, $http) {
		window.hub.disconnect();
		$scope.loading();
		var self = this;
		self.stats = [];
		self.data = {};
		$scope.kw = "";
		$scope.paginationConf = {
			currentPage: 1,
			itemsPerPage: 10,
			pagesLength: 25,
			perPageOptions: [1, 5, 10, 15, 20, 30, 40, 50, 100, 200],
			rememberPerPage: 'perPageItems',
			onChange: function() {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		};
		this.GetPageData = function(page, size) {
			$scope.loading();
			$http.post("/user/page", {
				page,
				size,
				kw: $scope.kw
			}).then(function(res) {
				$scope.paginationConf.totalItems = res.data.TotalCount;
				$("div[ng-table-pagination]").remove();
				self.tableParams = new NgTableParams({
					count: 50000
				}, {
					filterDelay: 0,
					dataset: res.data.Data
				});
				self.data = res.data.Data;
				Enumerable.From(res.data.Data).Select(e => e.Status).Distinct().ToArray().map(function(item, index, array) {
					self.stats.push({
						id: item,
						title: item
					});
				});
				self.stats = Enumerable.From(self.stats).Distinct().ToArray();
				$scope.loadingDone();
			});
		}
		var _timeout;
		$scope.search = function(kw) {
			if (_timeout) {
				$timeout.cancel(_timeout);
			}
			_timeout = $timeout(function() {
				$scope.kw = kw;
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
				_timeout = null;
			}, 500);
		}
		self.del = function(row) {
			swal({
				title: "确认删除这个用户吗？",
				text: row.Title,
				showCancelButton: true,
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "确定",
				cancelButtonText: "取消",
				showLoaderOnConfirm: true,
				animation: true,
				allowOutsideClick: false
			}).then(function() {
				$scope.request("/user/delete", {
					id: row.Id
				}, function(data) {
					window.notie.alert({
						type: 1,
						text: data.Message,
						time: 4
					});
				});
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}, function() {
			}).catch(swal.noop);
		}
		$scope.toggle= function(row) {
			$http.post("/user/lockuser", {
				id:row.Id,
				state:row.Locked
			}).then(function(res) {
				if (!res.data.Success) {
					self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
				}
			});
		}
	}]);