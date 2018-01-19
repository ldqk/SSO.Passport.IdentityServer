myApp.controller("log", ["$scope", "$http", function ($scope, $http) {
	window.hub.disconnect();
	$scope.getfiles= function() {
		$scope.request("/system/GetLogfiles", null, function(data) {
			$scope.files = data.Data;
		});
	}
	$scope.getfiles();
	$scope.view= function(file) {
		$scope.request("/system/catlog", { filename: file }, function (data) {
			swal({
				input: 'textarea',
				showCloseButton: true,
				width: 1000,
				confirmButtonColor: "#DD6B55",
				confirmButtonText: "确定",
				inputValue: data.Data,
				inputClass:"height700"
			});
		});
	}
	$scope.delete= function(file) {
		swal({
			title: '确定删除吗？',
			type: 'warning',
			showCancelButton: true,
			showCloseButton: true,
			confirmButtonColor: '#3085d6',
			cancelButtonColor: '#d33',
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			preConfirm: function() {
				return new Promise(function (resolve) {
					$scope.request("/system/deletefile", { filename: file }, function (res) {
						$scope.getfiles();
						resolve(res.Message);
					});
				});
			}
		}).then(function (msg) {
			swal(
				msg,
				'',
				'success'
			);
		}).catch(swal.noop);
	}
}]);
myApp.controller("file", ["$scope", "$http", function ($scope, $http) {
	window.hub.disconnect();
}]);
myApp.controller("task", ["$scope", "$http", function ($scope, $http) {
	window.hub.disconnect();
}]);
myApp.controller("swagger", ["$scope", "$http", function ($scope, $http) {
	window.hub.disconnect();
}]);