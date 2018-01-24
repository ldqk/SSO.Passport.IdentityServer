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
					return $ocLazyLoad.load([{
						files: ["/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
						cache: true
					},cpath + "/user.js"]);
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
		}).state("user-authority", {
			url: "/user/authority/:id",
			templateUrl: vpath + "/user/authority.html",
			controller: "userAuthority as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["https://cdn.bootcss.com/angular-ui-tree/2.22.6/angular-ui-tree.css","/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
						cache: true
					},cpath + "/user.js"]);
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
		}).state("group-apps", {
			url: "/group/apps/:id",
			templateUrl: vpath + "/group/apps.html",
			controller: "groupApps as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/group.js"]);
				}]
			}
		}).state("group-role", {
			url: "/group/role/:id",
			templateUrl: vpath + "/group/role.html",
			controller: "groupRoles as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/group.js"]);
				}]
			}
		}).state("group-user", {
			url: "/group/user/:id",
			templateUrl: vpath + "/group/user.html",
			controller: "groupUsers as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
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
		}).state("role-apps", {
			url: "/role/apps/:id",
			templateUrl: vpath + "/role/apps.html",
			controller: "roleApps as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/role.js"]);
				}]
			}
		}).state("role-group", {
			url: "/role/group/:id",
			templateUrl: vpath + "/role/group.html",
			controller: "roleGroups as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/role.js"]);
				}]
			}
		}).state("role-permission", {
			url: "/role/permission/:id",
			templateUrl: vpath + "/role/permission.html",
			controller: "rolePermissions as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/role.js"]);
				}]
			}
		}).state("role-user", {
			url: "/role/user/:id",
			templateUrl: vpath + "/role/user.html",
			controller: "roleUsers as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
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
		}).state("permission-access", {
			url: "/permission/access/:id",
			templateUrl: vpath + "/permission/access.html",
			controller: "permissionAccess as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/permission.js"]);
				}]
			}
		}).state("permission-apps", {
			url: "/permission/apps/:id",
			templateUrl: vpath + "/permission/apps.html",
			controller: "permissionApps as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/permission.js"]);
				}]
			}
		}).state("permission-menu", {
			url: "/permission/menu/:id",
			templateUrl: vpath + "/permission/menu.html",
			controller: "permissionMenus as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/permission.js"]);
				}]
			}
		}).state("permission-role", {
			url: "/permission/role/:id",
			templateUrl: vpath + "/permission/role.html",
			controller: "permissionRoles as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/permission.js"]);
				}]
			}
		}).state("permission-user", {
			url: "/permission/user/:id",
			templateUrl: vpath + "/permission/user.html",
			controller: "permissionUsers as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([{
						files: ["/Assets/semantic/semantic.css","https://cdn.bootcss.com/semantic-ui/2.2.13/semantic.min.js"],
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
		}).state("access-permission", {
			url: "/access/permission/:id",
			templateUrl: vpath + "/access/permission.html",
			controller: "accessPermission as list",
			resolve: {
				deps: ["$ocLazyLoad", function($ocLazyLoad) {
					return $ocLazyLoad.load([cpath + "/access.js"]);
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
		}).state("menu-permission", {
			url: "/menu/permission/:id",
			templateUrl: vpath + "/menu/permission.html",
			controller: "menuPermission as list",
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