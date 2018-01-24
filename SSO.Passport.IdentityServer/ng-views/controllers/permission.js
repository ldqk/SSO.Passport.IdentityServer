myApp.controller('permission', ["$timeout", "$state", "NgTableParams", "$scope", "$http", function($timeout, $state, NgTableParams, $scope, $http) {
		window.hub.disconnect();
	$scope.loading();
	$scope.permission = {};
	$scope.appid="";
	$scope.query="";
	$scope.init = function() {
		$scope.request("/app/getall",null, function(data) {
			$scope.apps=data.Data.concat([{AppId:"",AppName:"所有"}]);
			$scope.appid=$scope.apps[0].AppId;
			$('.ui.dropdown.apps').dropdown({
				onChange: function (value) {
					$scope.appid=value;
				sessionStorage.setItem("appid",value);
					$scope.request("/permission/GetAll", {
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
				$scope.appid=$scope.apps[0].AppId;
				$('.ui.dropdown.apps').dropdown("set selected", [$scope.apps[0].AppId]);
			},10);
		});
	}
	var sourceId, destId, index, parent, sourceIndex;
	$scope.treeOptions = {
		beforeDrop: function(e) {
			index = e.dest.index;
			if (e.dest.nodesScope.$parent.$modelValue) {
				parent = e.dest.nodesScope.$parent.$modelValue; //找出父级元素
			}
		},
		dropped: function(e) {
			var dest = e.dest.nodesScope;
			destId = dest.$id;
			var pid = dest.node ? dest.node.id : 0; //pid
			var prev = null;
			var next = null;
			if (index > sourceIndex) {
				next = dest.$modelValue[index + 1], prev = dest.$modelValue[index];
			} else if (index < sourceIndex) {
				next = dest.$modelValue[index], prev = dest.$modelValue[index - 1];
			} else {
				next = dest.$modelValue[index];
			}
			var current = e.source.nodeScope.$modelValue;
			if (destId == sourceId) {
				if (index == sourceIndex) {
					//位置没改变
					return;
				}
				//同级内改变位置，找出兄弟结点，排序号更新
				if (prev || next) {
					//有多个子节点
					if (next) {
						//console.log("自己：", current, "后一个元素：", next);
						current.ParentId = pid;
						current.Sort = next.Sort - 1;
						$scope.request("/permission/save", current, function(data) {
							window.notie.alert({
								type: 1,
								text: data.Message,
								time: 3
							});
						});
					} else if (prev) {
						//console.log("自己：", current, "前一个元素：", prev);
						current.ParentId = pid;
						current.Sort = prev.Sort + 1;
						$scope.request("/permission/save", current, function (data) {
							window.notie.alert({
								type: 1,
								text: data.Message,
								time: 3
							});
						});
					}
				}
			} else {
				//层级位置改变
				if (parent) {
					//非顶级元素
					//找兄弟结点
					next = dest.$modelValue[index], prev = dest.$modelValue[index - 1];
					if (prev || next) {
						//有多个子节点
						if (next) {
							//console.log("自己：", current, "后一个元素：", next);
							current.ParentId = parent.Id;
							current.Sort = next.Sort - 1;
							$scope.request("/permission/save", current, function (data) {
								window.notie.alert({
									type: 1,
									text: data.Message,
									time: 3
								});
							});
						} else if (prev) {
							//console.log("自己：", current, "前一个元素：", prev);
							current.ParentId = parent.Id;
							current.Sort = prev.Sort + 1;
							$scope.request("/permission/save", current, function (data) {
								window.notie.alert({
									type: 1,
									text: data.Message,
									time: 3
								});
							});
						}
					} else {
						//只有一个元素
						//console.log("自己：", current, "父亲元素：", parent);
						current.ParentId = parent.Id;
						current.Sort = parent.Sort * 10;
						$scope.request("/permission/save", current, function (data) {
							window.notie.alert({
								type: 1,
								text: data.Message,
								time: 3
							});
						});
					}
				} else {
					//顶级元素
					sourceIndex = e.source.nodesScope.$parent.index();
					if (index < sourceIndex) {
						next = dest.$modelValue[index + 1], prev = dest.$modelValue[index];
					} else {
						next = dest.$modelValue[index], prev = dest.$modelValue[index - 1];
					}
					//console.log("后一个元素：", next, "前一个元素：", prev, "自己：", current);
					if (next) {
						current.ParentId = pid;
						current.Sort = next.Sort - 1;
						$scope.request("/permission/save", current, function (data) {
							window.notie.alert({
								type: 1,
								text: data.Message,
								time: 3
							});
						});
					} else if (prev) {
						current.ParentId = pid;
						current.Sort = prev.Sort + 1;
						$scope.request("/permission/save", current, function (data) {
							window.notie.alert({
								type: 1,
								text: data.Message,
								time: 3
							});
						});
					}
				}
				parent = null;
			}
		},
		dragStart: function(e) {
			sourceId = e.dest.nodesScope.$id;
			sourceIndex = e.dest.index;
		}
	};
	$scope.findNodes = function () {
		$scope.request("/permission/GetAll", {
			appid:$scope.appid,
			kw:$scope.query
		}, function(data) {
			$scope.data = transData(data.Data, "Id", "ParentId", "nodes");
		});	
	};
	$scope.visible = function (item) {
		return true;
	};
	$scope.permission = {};
	$scope.newItem = function() {
		layer.open({
			type: 1,
			zIndex: 20,
			title: '修改权限信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.permission = {};
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
		$scope.permission.ParentId  = 0;
	};
	$scope.subpermission = {};

	$scope.closeAll = function() {
		layer.closeAll();
		setTimeout(function() {
			$("#modal").css("display", "none");
		}, 500);
	}
	$scope.newSubItem = function (scope) {
		layer.open({
			type: 1,
			zIndex: 20,
			title: '修改权限信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.permission = {};
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
		var nodeData = scope.$modelValue;
		$scope.subpermission = nodeData;
		if (nodeData.Url && nodeData.Url != "#") {
			swal("异常操作！", "权限【" + nodeData.GroupName + "】是一个有链接的权限，不能作为父级权限", "error");
			return false;
		}
		$scope.permission.Sort = (nodeData.Sort + nodeData.nodes.length + 1) * 10;
		$scope.permission.ParentId = nodeData.Id;
	};
	$scope.expandAll = function() {
		if ($scope.collapse) {
			$scope.$broadcast('angular-ui-tree:collapse-all');
		} else {
			$scope.$broadcast('angular-ui-tree:expand-all');
		}
		$scope.collapse = !$scope.collapse;
	};
	
	$scope.del = function(scope) {
		var model = scope.$nodeScope.$modelValue;
		var id = model.Id;
		swal({
			title: "确认删除这个权限吗？",
			text: model.GroupName,
			showCancelButton: true,
			showCloseButton: true,
			confirmButtonColor: "#DD6B55",
			confirmButtonText: "确定",
			cancelButtonText: "取消",
			showLoaderOnConfirm: true,
			animation: true,
			allowOutsideClick: false
		}).then(function() {
			$scope.request("/permission/delete", {
				id: id
			}, function(data) {
				window.notie.alert({
					type: 1,
					text: data.Message,
					time: 4
				});
				scope.remove();
			});
		}, function() {
		}).catch(swal.noop);
	}
	
	$scope.edit= function(permission) {
		$scope.permission = permission;
		layer.open({
			type: 1,
			zIndex: 20,
			title: '修改权限信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.permission = permission;
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
	}
	
	$scope.submit = function (permission) {
		if (permission.Id) {
			//修改
			$scope.request("/permission/save", permission, function (data) {
				swal(data.Message, null, 'info');
				$scope.permission = {};
				$scope.closeAll();
			});
		}else {
			//添加
			permission.appid=$scope.appid;
			if (permission.ParentId) {
				//添加子权限
				$scope.subpermission.nodes.push(permission);
			} else {
				//添加主权限
				$scope.data.push(permission);
			}
			$scope.request("/permission/save", permission, function (data) {
				window.notie.alert({
					type: data.Success?1:3,
					text: data.Message,
					time: 3
				});
				$scope.findNodes();
				$scope.permission = {};
				$scope.closeAll();
				//$scope.init();
			});
		}
	}
	$scope.init();
	}]);
myApp.controller('permissionApps', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/permission/get/"+$scope.id,null, function(data) {
		$scope.permission=data.Data;
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
			$http.post("/permission/myapps", {
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
	this.addApps= function(row) {
		$scope.request("/permission/Addapps", {
			id:$scope.id,
			aids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeApps= function(row) {
		$scope.request("/permission/removeapps", {
			id:$scope.id,
			aids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('permissionRoles', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/permission/get/"+$scope.id,null, function(data) {
		$scope.permission=data.Data;
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
			$http.post("/permission/myroles", {
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
		$scope.request("/permission/AddRoles", {
			id:$scope.id,
			rids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeRoles= function(row) {
		$scope.request("/permission/RemoveRoles", {
			id:$scope.id,
			rids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('permissionUsers', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams","$location", function($timeout, $state, $scope, $http,$stateParams,NgTableParams,$location) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.loading();
	var self = this;
		self.stats = [];
		self.data = {};
		$scope.kw = "";
	$scope.request("/permission/get/"+$scope.id,null, function(data) {
		$scope.permission=data.Data;
	});
	
	$scope.request("/app/getall",null, function(data) {
		$scope.apps=data.Data.concat([{AppId:"",AppName:"所有"}]);
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
			$http.post("/permission/myusers", {
				id:$scope.id,
				page,
				size,
				kw: $scope.kw,
				appid:$scope.appid
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
		$scope.request("/permission/AddUsers", {
			id:$scope.id,
			uids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removePermissions= function(row) {
		$scope.request("/permission/RemoveUsers", {
			id:$scope.id,
			uids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	$scope.toggle= function(row) {
		$http.post("/permission/ToggleState", {
			id:$scope.id,
			uid:row.Id,
			state:row.HasPermission
		}).then(function(res) {
			if (!res.data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('permissionAccess', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/permission/get/"+$scope.id,null, function(data) {
		$scope.permission=data.Data;
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
			$http.post("/permission/MyControls", {
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
	this.addAccess= function(row) {
		$scope.request("/permission/AddControls", {
			id:$scope.id,
			cids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeAccess= function(row) {
		$scope.request("/permission/RemoveControls", {
			id:$scope.id,
			cids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('permissionMenus', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/permission/get/"+$scope.id,null, function(data) {
		$scope.permission=data.Data;
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
			$http.post("/permission/MyMenus", {
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
	this.addMenu= function(row) {
		$scope.request("/permission/AddMenus", {
			id:$scope.id,
			mids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeMenu= function(row) {
		$scope.request("/permission/RemoveMenus", {
			id:$scope.id,
			mids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);