myApp.controller('group', ["$timeout", "$state", "NgTableParams", "$scope", "$http", function($timeout, $state, NgTableParams, $scope, $http) {
		window.hub.disconnect();
	$scope.loading();
	$scope.group = {};
	$scope.appid="";
	$scope.query="";
	$scope.init = function() {
		$scope.request("/app/getall",null, function(data) {
			$scope.apps=data.Data.concat([{AppId:"",AppName:"所有"}]);
			$scope.appid=$scope.apps[0].AppId;
			$('.ui.dropdown.apps').dropdown({
				onChange: function (value) {
					$scope.appid=value;
					$scope.request("/group/GetAll", {
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
						$scope.request("/group/save", current, function(data) {
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
						$scope.request("/group/save", current, function (data) {
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
							$scope.request("/group/save", current, function (data) {
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
							$scope.request("/group/save", current, function (data) {
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
						$scope.request("/group/save", current, function (data) {
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
						$scope.request("/group/save", current, function (data) {
							window.notie.alert({
								type: 1,
								text: data.Message,
								time: 3
							});
						});
					} else if (prev) {
						current.ParentId = pid;
						current.Sort = prev.Sort + 1;
						$scope.request("/group/save", current, function (data) {
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
		$scope.request("/group/GetAll", {
			appid:$scope.appid,
			kw:$scope.query
		}, function(data) {
			$scope.data = transData(data.Data, "Id", "ParentId", "nodes");
		});	
	};
	$scope.visible = function (item) {
		return true;
	};
	$scope.group = {};
	$scope.newItem = function() {
		layer.open({
			type: 1,
			zIndex: 20,
			title: '修改用户组信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.group = {};
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
		var nodeData = $scope.data[$scope.data.length - 1];
		$scope.group.Sort = nodeData.Sort + (nodeData.nodes.length + 1) * 10;
		$scope.group.ParentId  = 0;
	};
	$scope.subgroup = {};

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
			title: '修改用户组信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.group = {};
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
		var nodeData = scope.$modelValue;
		$scope.subgroup = nodeData;
		if (nodeData.Url && nodeData.Url != "#") {
			swal("异常操作！", "用户组【" + nodeData.GroupName + "】是一个有链接的用户组，不能作为父级用户组", "error");
			return false;
		}
		$scope.group.Sort = (nodeData.Sort + nodeData.nodes.length + 1) * 10;
		$scope.group.ParentId = nodeData.Id;
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
			title: "确认删除这个用户组吗？",
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
			$scope.request("/group/delete", {
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
	
	$scope.edit= function(group) {
		$scope.group = group;
		layer.open({
			type: 1,
			zIndex: 20,
			title: '修改用户组信息',
			area: (window.screen.width > 360 ? 360 : window.screen.width) + 'px',// '340px'], //宽高
			content: $("#modal"),
			success: function(layero, index) {
				$scope.group = group;
			},
			end: function() {
				$("#modal").css("display", "none");
			}
		});
	}
	
	$scope.submit = function (group) {
		if (group.Id) {
			//修改
			$scope.request("/group/save", group, function (data) {
				swal(data.Message, null, 'info');
				$scope.group = {};
				$scope.closeAll();
			});
		}else {
			//添加
			group.appid=$scope.appid;
			if (group.ParentId == 0) {
				//添加主用户组
				var nodeData = $scope.data[$scope.data.length - 1];
				group.Sort = nodeData.Sort + (nodeData.nodes.length + 1) * 10;
				$scope.data.push(group);
			} else {
				//添加子用户组
				$scope.subgroup.nodes.push(group);
			}
			$scope.request("/group/save", group, function (data) {
				window.notie.alert({
					type: data.Success?1:3,
					text: data.Message,
					time: 3
				});
				$scope.findNodes();
				$scope.group = {};
				$scope.closeAll();
				$scope.init();
			});
		}
	}
	$scope.init();
	}]);