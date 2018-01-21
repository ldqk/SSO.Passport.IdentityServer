myApp.controller('apps', ["$timeout", "$state", "NgTableParams", "$scope", "$http", function($timeout, $state, NgTableParams, $scope, $http) {
		window.hub.disconnect();
		$scope.loading();
		var self = this;
		self.stats = [];
		self.data = {};
		$scope.kw = "";
		$scope.paginationConf = {
			currentPage: 1,
			//totalItems: $scope.total,
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
			$http.post("/app/pagedata", {
				page,
				size,
				kw: $scope.kw
			}).then(function(res) {
				//$scope.paginationConf.currentPage = page;
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
		$scope.add = function() {
			swal({
				title: "添加应用",
				text: "请输入应用名:",
				input: "text",
				showCancelButton: true,
				closeOnConfirm: false,
				animation: "slide-from-top",
				confirmButtonText: "确认",
				cancelButtonText: "取消",
				inputPlaceholder: "请输入应用名",
				preConfirm: function(inputValue) {
					return new Promise(function(resolve, reject) {
						if (inputValue === false) {
							return false;
						}
						if (inputValue === "") {
							swal("应用名不能为空!", "", "error");
							return false;
						}
						$scope.request("/app/Add", {
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
				swal("添加成功!", "已新增应用：" + data.inputValue, "success");
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
				title: "确认删除这个应用吗？",
				text: row.Title,
				showCancelButton: true,
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "确定",
				cancelButtonText: "取消",
				showLoaderOnConfirm: true,
				animation: true,
				allowOutsideClick: false
			}).then(function() {
				$scope.request("/app/delete", {
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
				title: "修改应用名",
				html:'<div class="input-group"><span class="input-group-addon">应用名</span><input id="name" class="form-control" autofocus placeholder="应用名" value="'+row.AppName+'"></div>',
				showCancelButton: true,
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "确定",
				cancelButtonText: "取消",
				showLoaderOnConfirm: true,
				animation: true,
				allowOutsideClick: false
			}).then(function() {
				$scope.request("/app/update", {
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
			if (row.Preset) {
				return ;
			}
			$http.post("/app/togglestate", {
				id:row.Id,
				state:row.Available
			}).then(function(res) {
				if (!res.data.Success) {
					self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
				}
			});
		}
	}]);