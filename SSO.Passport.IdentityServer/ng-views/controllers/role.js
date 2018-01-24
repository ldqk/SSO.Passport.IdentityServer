myApp.controller('role', ["$timeout", "$state", "NgTableParams", "$scope", "$http", function($timeout, $state, NgTableParams, $scope, $http) {
		window.hub.disconnect();
	$scope.loading();
	$scope.role = {};
	$scope.appid="";
	$scope.query="";
	$scope.init = function() {
		$scope.request("/app/getall",null, function(data) {
			$scope.apps = data.Data.concat([{AppId:"",AppName:"所有"}]);
			$scope.appid=$scope.apps[0].AppId;
			$('.ui.dropdown.apps').dropdown({
				onChange: function (value) {
					$scope.appid=value;
					$scope.request("/role/GetAll", {
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
						$scope.request("/role/save", current, function(data) {
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
						$scope.request("/role/save", current, function (data) {
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
							$scope.request("/role/save", current, function (data) {
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
							$scope.request("/role/save", current, function (data) {
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
						$scope.request("/role/save", current, function (data) {
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
						$scope.request("/role/save", current, function (data) {
							window.notie.alert({
								type: 1,
								text: data.Message,
								time: 3
							});
						});
					} else if (prev) {
						current.ParentId = pid;
						current.Sort = prev.Sort + 1;
						$scope.request("/role/save", current, function (data) {
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
		$scope.request("/role/GetAll", {
			appid:$scope.appid,
			kw:$scope.query
		}, function(data) {
			$scope.data = transData(data.Data, "Id", "ParentId", "nodes");
		});	
	};
	$scope.visible = function (item) {
		return true;
	};
	$scope.role = {};
	$scope.newItem = function() {
		layer.open({
			type: 1,
			zIndex: 20,
			title: '修改角色信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.role = {};
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
		var nodeData = $scope.data[$scope.data.length - 1];
		$scope.role.Sort = nodeData.Sort + (nodeData.nodes.length + 1) * 10;
		$scope.role.ParentId  = 0;
	};
	$scope.subrole = {};

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
			title: '修改角色信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.role = {};
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
		var nodeData = scope.$modelValue;
		$scope.subrole = nodeData;
		if (nodeData.Url && nodeData.Url != "#") {
			swal("异常操作！", "角色【" + nodeData.GroupName + "】是一个有链接的角色，不能作为父级角色", "error");
			return false;
		}
		$scope.role.Sort = (nodeData.Sort + nodeData.nodes.length + 1) * 10;
		$scope.role.ParentId = nodeData.Id;
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
			title: "确认删除这个角色吗？",
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
			$scope.request("/role/delete", {
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
	
	$scope.edit= function(role) {
		$scope.role = role;
		layer.open({
			type: 1,
			zIndex: 20,
			title: '修改角色信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.role = role;
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
	}
	
	$scope.submit = function (role) {
		if (role.Id) {
			//修改
			$scope.request("/role/save", role, function (data) {
				swal(data.Message, null, 'info');
				$scope.role = {};
				$scope.closeAll();
			});
		}else {
			//添加
			role.appid=$scope.appid;
			if (role.ParentId) {
				//添加子角色
				$scope.subrole.nodes.push(role);
			} else {
				//添加主角色
				$scope.data.push(role);
			}
			$scope.request("/role/save", role, function (data) {
				window.notie.alert({
					type: data.Success?1:3,
					text: data.Message,
					time: 3
				});
				$scope.findNodes();
				$scope.role = {};
				$scope.closeAll();
				//$scope.init();
			});
		}
	}
	$scope.init();
	}]);
myApp.controller('roleApps', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/role/get/"+$scope.id,null, function(data) {
		$scope.role=data.Data;
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
			$http.post("/role/myapps", {
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
		$scope.request("/role/Addapps", {
			id:$scope.id,
			aids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeApps= function(row) {
		$scope.request("/role/removeapps", {
			id:$scope.id,
			aids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('rolePermissions', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/role/get/"+$scope.id,null, function(data) {
		$scope.role=data.Data;
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
			$http.post("/role/mypermissions", {
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
		$scope.request("/role/Addpermissions", {
			id:$scope.id,
			pids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removePermissions= function(row) {
		$scope.request("/role/Removepermissions", {
			id:$scope.id,
			pids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('roleGroups', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/role/get/"+$scope.id,null, function(data) {
		$scope.role=data.Data;
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
			$http.post("/role/mygroups", {
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
	this.addRole= function(row) {
		$scope.request("/role/AddGroups", {
			id:$scope.id,
			gids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeRole= function(row) {
		$scope.request("/role/RemoveGroups", {
			id:$scope.id,
			gids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	$scope.toggle= function(row) {
		$http.post("/role/ToggleState", {
			id:$scope.id,
			gid:row.Id,
			state:row.HasRole
		}).then(function(res) {
			if (!res.data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);
myApp.controller('roleUsers', ["$timeout", "$state", "$scope", "$http","$stateParams","NgTableParams", function($timeout, $state, $scope, $http,$stateParams,NgTableParams) {
	window.hub.disconnect();
	$scope.id=$stateParams.id;
	$scope.request("/role/get/"+$scope.id,null, function(data) {
		$scope.role=data.Data;
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
			$http.post("/role/myusers", {
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
	this.addUser= function(row) {
		$scope.request("/role/AddUsers", {
			id:$scope.id,
			uids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
	this.removeUser= function(row) {
		$scope.request("/role/RemoveUsers", {
			id:$scope.id,
			uids:row.Id
		}, function(data) {
			if (data.Success) {
				self.GetPageData($scope.paginationConf.currentPage, $scope.paginationConf.itemsPerPage);
			}
		});
	}
}]);