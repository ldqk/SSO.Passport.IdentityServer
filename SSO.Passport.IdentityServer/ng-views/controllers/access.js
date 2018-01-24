myApp.controller('access', ["$timeout", "$state", "NgTableParams", "$scope", "$http","$location", function($timeout, $state, NgTableParams, $scope, $http,$location) {
	window.hub.disconnect();
	$scope.loading();
	var self = this;
	self.stats = [];
	self.data = {};
	$scope.acl = {HttpMethod:null}
	$scope.kw = "";
	$scope.request("/app/getall",null, function(data) {
		$scope.apps=data.Data;
		$scope.appid=$location.search()['appid']||sessionStorage.getItem("appid")||$scope.apps[0].AppId;
		$('.ui.dropdown.apps').dropdown({
			onChange: function (value) {
				$scope.appid=value;
				sessionStorage.setItem("appid",value);
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			},
			message: {
				maxSelections: '最多选择 {maxCount} 项',
				noResults: '无搜索结果！'
			}
		});
		$timeout(function() {
			$scope.appid=$location.search()['appid']||sessionStorage.getItem("appid")||$scope.apps[0].AppId;
			$('.ui.dropdown.apps').dropdown("set selected", [$scope.appid]);
		},10);
	});
	$scope.request("/access/gethttpmethod",null, function(data) {
			$scope.HttpMethod=data.Data;
		$scope.acl.HttpMethod=data.Data[0].value;
		});
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
			$http.post("/access/pagedata", {
				appid:$scope.appid,
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
		$scope.add = function() {
			layer.open({
				type: 1,
				zIndex: 20,
				title: '修改菜单信息',
				area: (window.screen.width > 600 ? 600 : window.screen.width) + 'px',// '340px'], //宽高
				content: $("#modal"),
				success: function(layero, index) {
					$scope.menu = {};
				},
				end: function() {
					$("#modal").css("display", "none");
				}
			});
		}
	$scope.submit= function(acl) {
		acl.ClientAppId = _.find($scope.apps, {AppId:$scope.appid}).Id;
		if (acl.Name.length<=0) {
			window.notie.alert({
				type: 3,
				text: "访问控制名不能为空",
				time: 4
			});
		}
		if (acl.HttpMethod) {
			$scope.request("/access/add",acl, function(res) {
				if (res.Success) {
					$scope.closeAll();
					self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
				} else {
					window.notie.alert({
						type: 3,
						text: res.Message,
						time: 4
					});									
				}
			});
		} else {
			window.notie.alert({
				type: 3,
				text: "请求方式不能为空",
				time: 4
			});
		}
	}
		$scope.closeAll = function() {
			layer.closeAll();
			setTimeout(function() {
				$("#modal").css("display", "none");
			}, 500);
		}

		self.del = function(row) {
			swal({
				title: "确认删除这个访问控制器吗？",
				text: row.Title,
				showCancelButton: true,
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "确定",
				cancelButtonText: "取消",
				showLoaderOnConfirm: true,
				animation: true,
				allowOutsideClick: false
			}).then(function() {
				$scope.request("/access/delete", {
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
		self.edit = function(row) {
			swal({
				title: "修改访问控制器名",
				html:'<div class="input-group"><span class="input-group-addon">访问控制器名</span><input id="name" class="form-control" autofocus placeholder="访问控制器名" value="'+row.AppName+'"></div>',
				showCancelButton: true,
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "确定",
				cancelButtonText: "取消",
				showLoaderOnConfirm: true,
				animation: true,
				allowOutsideClick: false
			}).then(function() {
				$scope.request("/access/update", {
					id: row.Id,
					name:$("#name").val()
				}, function(data) {
					window.notie.alert({
						type: data.Success?1:3,
						text: data.Message,
						time: 4
					});
				});
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}, function() {
			}).catch(swal.noop);
		}
		$scope.toggle= function(row) {
			$http.post("/access/togglestate", {
				id:row.Id,
				state:row.IsAvailable
			}).then(function(res) {
				if (!res.data.Success) {
					self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
				}
			});
		}
	}]);
myApp.controller('accessPermission', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/access/get/"+$scope.id,null, function(data) {
		$scope.access=data.Data;
	});
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
		$scope.paginationConf2 = {
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
			$http.post("/access/mypermissions", {
				id:$scope.id,
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
					dataset: res.data.Data.not
				});
				self.tableParams2 = new NgTableParams({
					count: 50000
				}, {
					filterDelay: 0,
					dataset: res.data.Data.my
				});
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
	this.addPermissions= function(row) {
		$scope.request("/access/AssignPermission", {
			id:$scope.id,
			pid:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removePermissions= function(row) {
		$scope.request("/access/RemovePermission", {
			id:$scope.id,
			pid:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);