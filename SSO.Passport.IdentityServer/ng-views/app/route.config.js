"use strict";
myApp.config(["$stateProvider", "$urlRouterProvider", "$locationProvider",function($stateProvider, $urlRouterProvider, $locationProvider) {
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
		}).state("user", {
			url: "/user",
			templateUrl: vpath + "/user/list.html",
			controller: "user as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/user.js"]);
				}]
			}
		}).state("user-apps", {
			url: "/user/apps/:id",
			templateUrl: vpath + "/user/apps.html",
			controller: "userApps as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/user.js"]);
				}]
			}
		}).state("user-groups", {
			url: "/user/groups/:id",
			templateUrl: vpath + "/user/groups.html",
			controller: "userGroups as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/user.js"]);
				}]
			}
		}).state("user-roles", {
			url: "/user/roles/:id",
			templateUrl: vpath + "/user/roles.html",
			controller: "userRoles as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/user.js"]);
				}]
			}
		}).state("user-permissions", {
			url: "/user/permissions/:id",
			templateUrl: vpath + "/user/permissions.html",
			controller: "userPermissions as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/user.js"]);
				}]
			}
		}).state("apps", {
			url: "/apps",
			templateUrl: vpath + "/apps/list.html",
			controller: "apps as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/clientApps.js"]);
				}]
			}
		}).state("app-user", {
			url: "/apps/user/:id",
			templateUrl: vpath + "/apps/user.html",
			controller: "appUser as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/clientApps.js"]);
				}]
			}
		}).state("app-group", {
			url: "/apps/group/:id",
			templateUrl: vpath + "/apps/group.html",
			controller: "appGroup as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/clientApps.js"]);
				}]
			}
		}).state("app-role", {
			url: "/apps/role/:id",
			templateUrl: vpath + "/apps/role.html",
			controller: "appRole as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/clientApps.js"]);
				}]
			}
		}).state("app-permission", {
			url: "/apps/permission/:id",
			templateUrl: vpath + "/apps/permission.html",
			controller: "appPermission as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/clientApps.js"]);
				}]
			}
		}).state("group", {
			url: "/group",
			templateUrl: vpath + "/group/list.html",
			controller: "group as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["https://cdn.bootcss.com/angular-ui-tree/2.22.6/angular-ui-tree.css","/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
						cache: true
					},cpath + "/group.js"]);
				}]
			}
		}).state("role", {
			url: "/role",
			templateUrl: vpath + "/role/list.html",
			controller: "role as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["https://cdn.bootcss.com/angular-ui-tree/2.22.6/angular-ui-tree.css","/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
						cache: true
					},cpath + "/role.js"]);
				}]
			}
		}).state("permission", {
			url: "/permission",
			templateUrl: vpath + "/permission/list.html",
			controller: "permission as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["https://cdn.bootcss.com/angular-ui-tree/2.22.6/angular-ui-tree.css","/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
						cache: true
					},cpath + "/permission.js"]);
				}]
			}
		}).state("access", {
			url: "/access",
			templateUrl: vpath + "/access/list.html",
			controller: "access as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
						cache: true
					},cpath + "/access.js"]);
				}]
			}
		}).state("menu", {
			url: "/menu",
			templateUrl: vpath + "/menu/list.html",
			controller: "menu as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["https://cdn.bootcss.com/angular-ui-tree/2.22.6/angular-ui-tree.css","/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
						cache: true
					},cpath + "/menu.js"]);
				}]
			}
		})
	}]);