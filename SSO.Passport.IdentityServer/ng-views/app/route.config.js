"use strict";
myApp.config(["$stateProvider", "$urlRouterProvider", "$locationProvider",
	function($stateProvider, $urlRouterProvider, $locationProvider) {
		$locationProvider.hashPrefix('');
		$urlRouterProvider.otherwise("/home");
		var vpath = "/ng-views/views";
		var cpath = "/ng-views/controllers";
		$stateProvider.state("dashboard", {
			url: "/home",
			templateUrl: vpath + "/dashboard.html",
			controller: "dashboard",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["/Scripts/boost.js"],
						cache: true
					}, cpath + "/dashboard.js"]);
				}]
			}
		}).state("system-log", {
			url: "/system/log",
			templateUrl: vpath + "/system/log.html",
			controller: "log",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/system.js"]);
				}]
			}
		}).state("filemanager", {
			url: "/system/file",
			templateUrl: vpath + "/system/file.html",
			controller: "file",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/system.js"]);
				}]
			}
		}).state("taskcenter", {
			url: "/system/task",
			templateUrl: vpath + "/system/task.html",
			controller: "task",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/system.js"]);
				}]
			}
		}).state("swagger", {
			url: "/system/swagger",
			templateUrl: vpath + "/system/swagger.html",
			controller: "swagger",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/system.js"]);
				}]
			}
		}).state("loginrecord", {
			url: "/loginrecord",
			templateUrl: vpath + "/loginrecord.html",
			controller: "loginrecord as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/loginrecord.js"]);
				}]
			}
		})
	}]);