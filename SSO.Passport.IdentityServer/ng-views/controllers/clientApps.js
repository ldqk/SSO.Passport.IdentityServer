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
				html:'<div class="input-group"><span class="input-group-addon">子系统应用名</span><input id="name" class="form-control" autofocus placeholder="子系统应用名"></div>' +
						'<div class="input-group"><span class="input-group-addon">子系统域名</span><input id="domain" class="form-control" placeholder="子系统域名"></div>',
				showCancelButton: true,
				closeOnConfirm: false,
				animation: "slide-from-top",
				confirmButtonText: "确认",
				cancelButtonText: "取消",
				inputPlaceholder: "请输入应用名",
				preConfirm: function(inputValue) {
					return new Promise(function(resolve, reject) {
						if ($("#name").val().length<=0) {
							reject("应用程序名称不能为空！");
						}
						if ($("#domain").val().length<=0) {
							reject("应用程序域名不能为空！");
						}
						$scope.request("/app/Add", {
							name: $("#name").val(),
							domain:$("#domain").val()
						}, function(data) {
							data.inputValue = $("#name").val();
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
myApp.controller('appUser', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/app/get/"+$scope.id,null, function(data) {
		$scope.app=data.Data;
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
			$http.post("/app/myusers", {
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
					dataset: res.data.Data.notMyUsers
				});
				self.tableParams2 = new NgTableParams({
					count: 50000
				}, {
					filterDelay: 0,
					dataset: res.data.Data.myUsers
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
	this.addUser= function(row) {
		$scope.request("/app/AddUsers", {
			id:$scope.id,
			uids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeUser= function(row) {
		$scope.request("/app/RemoveUsers", {
			id:$scope.id,
			uids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('appGroup', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/app/get/"+$scope.id,null, function(data) {
		$scope.app=data.Data;
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
			$http.post("/app/mygroups", {
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
	this.addGroups= function(row) {
		$scope.request("/app/AddUserGroups", {
			id:$scope.id,
			gids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeGroups= function(row) {
		$scope.request("/app/RemoveUserGroups", {
			id:$scope.id,
			gids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('appRole', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/app/get/"+$scope.id,null, function(data) {
		$scope.app=data.Data;
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
			$http.post("/app/myroles", {
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
	this.addRoles= function(row) {
		$scope.request("/app/AddRoles", {
			id:$scope.id,
			rids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeRoles= function(row) {
		$scope.request("/app/RemoveRoles", {
			id:$scope.id,
			rids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('appPermission', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/app/get/"+$scope.id,null, function(data) {
		$scope.app=data.Data;
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
		this.GetPageData = function(page, size) {
			$http.post("/app/mypermissions", {
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
		$scope.request("/app/Addpermissions", {
			id:$scope.id,
			pids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removePermissions= function(row) {
		$scope.request("/app/Removepermissions", {
			id:$scope.id,
			pids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('appDetails', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.loading();
	var pending=false;
	var self = this;
	$scope.request("/app/get/"+$scope.id,null, function(data) {
		$scope.app=data.Data;
			self.GetUserPageData($scope.paginationConf_user.currentPage, $scope.paginationConf_user.itemsPerPage);
			self.GetGroupPageData($scope.paginationConf_group.currentPage, $scope.paginationConf_group.itemsPerPage);
			self.GetRolePageData($scope.paginationConf_role.currentPage, $scope.paginationConf_role.itemsPerPage);
			self.GetPermissionPageData($scope.paginationConf_permission.currentPage, $scope.paginationConf_permission.itemsPerPage);
	});
	self.stats = [];
	self.data = {};
	$scope.kw = "";
	$scope.paginationConf_user = {
		currentPage: 1,
		itemsPerPage: 10,
		pagesLength: 25,
		perPageOptions: [1, 5, 10, 15, 20, 30, 40, 50, 100, 200],
		//rememberPerPage: 'perPageItems',
		onChange: function() {
			if(pending)return;
			self.GetUserPageData($scope.paginationConf_user.currentPage, $scope.paginationConf_user.itemsPerPage);
		}
	};
	this.GetUserPageData = function(page, size) {
		pending=true;
		$http.post("/user/page", {
			appid:$scope.app.AppId,
			page,
			size,
			kw: $scope.kw
		}).then(function(res) {
			$scope.paginationConf_user.totalItems = res.data.TotalCount;
			$("div[ng-table-pagination]").remove();
			self.users = new NgTableParams({
				count: 50000
			}, {
				filterDelay: 0,
				dataset: res.data.Data
			});
			$scope.loadingDone();
			pending=false;
		});
	}

	$scope.paginationConf_group = {
		currentPage: 1,
		itemsPerPage: 10,
		pagesLength: 25,
		perPageOptions: [1, 5, 10, 15, 20, 30, 40, 50, 100, 200],
		//rememberPerPage: 'perPageItems',
		onChange: function() {
			if(pending)return;
			self.GetGroupPageData($scope.paginationConf_group.currentPage, $scope.paginationConf_group.itemsPerPage);
		}
	};
	this.GetGroupPageData = function(page, size) {
		pending=true;
		$http.post("/group/pagedata", {
			appid:$scope.app.AppId,
			page,
			size,
			kw: $scope.kw
		}).then(function(res) {
			$scope.paginationConf_group.totalItems = res.data.TotalCount;
			$("div[ng-table-pagination]").remove();
			self.groups = new NgTableParams({
				count: 50000
			}, {
				filterDelay: 0,
				dataset: res.data.Data
			});
			$scope.loadingDone();
			pending=false;
		});
	}

	$scope.paginationConf_role = {
		currentPage: 1,
		itemsPerPage: 10,
		pagesLength: 25,
		perPageOptions: [1, 5, 10, 15, 20, 30, 40, 50, 100, 200],
		//rememberPerPage: 'perPageItems',
		onChange: function() {
			if(pending)return;
			self.GetRolePageData($scope.paginationConf_role.currentPage, $scope.paginationConf_role.itemsPerPage);
		}
	};
	this.GetRolePageData = function(page, size) {
		pending=true;
		$http.post("/role/pagedata", {
			appid:$scope.app.AppId,
			page,
			size,
			kw: $scope.kw
		}).then(function(res) {
			$scope.paginationConf_role.totalItems = res.data.TotalCount;
			$("div[ng-table-pagination]").remove();
			self.roles = new NgTableParams({
				count: 50000
			}, {
				filterDelay: 0,
				dataset: res.data.Data
			});
			$scope.loadingDone();
			pending=false;
		});
	}

	$scope.paginationConf_permission = {
		currentPage: 1,
		itemsPerPage: 10,
		pagesLength: 25,
		perPageOptions: [1, 5, 10, 15, 20, 30, 40, 50, 100, 200],
		//rememberPerPage: 'perPageItems',
		onChange: function() {
			if(pending)return;
			self.GetPermissionPageData($scope.paginationConf_permission.currentPage, $scope.paginationConf_permission.itemsPerPage);
		}
	};
	this.GetPermissionPageData = function(page, size) {
		pending=true;
		$http.post("/permission/pagedata", {
			appid:$scope.app.AppId,
			page,
			size,
			kw: $scope.kw
		}).then(function(res) {
			$scope.paginationConf_permission.totalItems = res.data.TotalCount;
			$("div[ng-table-pagination]").remove();
			self.permissions = new NgTableParams({
				count: 50000
			}, {
				filterDelay: 0,
				dataset: res.data.Data
			});
			$scope.loadingDone();
			pending=false;
		});
	}

	var _timeout;
	$scope.search = function(kw) {
		if (_timeout) {
			$timeout.cancel(_timeout);
		}
		_timeout = $timeout(function() {
			$scope.kw = kw;
			self.GetUserPageData($scope.paginationConf_user.currentPage, $scope.paginationConf_user.itemsPerPage);
			self.GetGroupPageData($scope.paginationConf_group.currentPage, $scope.paginationConf_group.itemsPerPage);
			self.GetRolePageData($scope.paginationConf_role.currentPage, $scope.paginationConf_role.itemsPerPage);
			self.GetPermissionPageData($scope.paginationConf_permission.currentPage, $scope.paginationConf_permission.itemsPerPage);
			_timeout = null;
		}, 500);
	}
}]);