myApp.controller('access', ["$timeout", "$state", "NgTableParams", "$scope", "$http","$stateParams", function($timeout, $state, NgTableParams, $scope, $http,$stateParams) {
		window.hub.disconnect();
		$scope.loading();
		var self = this;
		self.stats = [];
		self.data = {};
		$scope.kw = "";
	$scope.request("/app/getall",null, function(data) {
			$scope.apps=data.Data;
			$scope.appid=$stateParams.appid||$scope.apps[0].AppId;
			$('.ui.dropdown.apps').dropdown({
				onChange: function (value) {
					$scope.appid=value;
					$scope.request("/menu/GetAll", {
						appid:value
					}, function(data) {
						$scope.data = transData(data.Data, "Id", "ParentId", "nodes");
					});	
				},
				message: {
					maxSelections: '最多选择 {maxCount} 项',
					noResults: '无搜索结果！'
				}
			});
			$timeout(function() {
				$scope.appid=$stateParams.appid||$scope.apps[0].AppId;
				$('.ui.dropdown.apps').dropdown("set selected", [$scope.appid]);
			},10);
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
			swal({
				title: "添加访问控制器",
				text: "请输入访问控制器名:",
				input: "text",
				showCancelButton: true,
				closeOnConfirm: false,
				animation: "slide-from-top",
				confirmButtonText: "确认",
				cancelButtonText: "取消",
				inputPlaceholder: "请输入访问控制器名",
				preConfirm: function(inputValue) {
					return new Promise(function(resolve, reject) {
						if (inputValue === false) {
							return false;
						}
						if (inputValue === "") {
							swal("访问控制器名不能为空!", "", "error");
							return false;
						}
						$scope.request("/access/Add", {
							name: inputValue
						}, function(data) {
							data.inputValue = inputValue;
							if (data.Success) {
								resolve(data);
							} else {
								reject(data.Message);
							}
						});
					});
				},
				allowOutsideClick: false
			}).then(function(data) {
				swal("添加成功!", "已新增访问控制器：" + data.inputValue, "success");
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}, function(error) {
				swal("操作被取消!", "未做任何更改!", "info");
			}).catch(swal.noop);
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