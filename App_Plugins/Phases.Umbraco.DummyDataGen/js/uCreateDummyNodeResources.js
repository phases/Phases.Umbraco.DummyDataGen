angular.module("umbraco.resources")
    .factory("uCreateDummyNodeResources", function ($q, $http, umbRequestHelper) {
        return {

            getAllowedDocTypes: function (parentId) {
                return umbRequestHelper.resourcePromise(
                    $http.get("/umbraco/backoffice/UmbracoApi/ContentType/GetAllowedChildren?contentId=" + parentId), "Failed to retrieve all Person data");
            },
            process: function (vm) {
                return $http({
                    method: "POST",
                    url: "/umbraco/backoffice/dummy/DummyContent/CreateNodeWithDummyContents",
                    data: JSON.stringify(vm), // Strinify your object
                    headers: { 'Content-Type': 'application/json' }
                });
            }
        };

    });