angular.module("umbraco")
    .controller("UCreateDummyNodeController",
        function ($scope, uCreateDummyNodeResources, $window, navigationService) {

            var vm = this;
            vm.docTypes = [];
            vm.getAllowedDocTypes = getAllowedDocTypes;
            vm.createDummyNode = createDummyNode;
            vm.currentNode = $scope.currentNode;
            vm.Loading = false;
            function getAllowedDocTypes(event) {
                if ($scope.currentNode != undefined) {
                    var currentNodeId = $scope.currentNode.id;
                    uCreateDummyNodeResources.getAllowedDocTypes(currentNodeId).then(function (response) {
                        if (response !== undefined && response.length > 0) {
                            vm.docTypes = response;
                        }
                    });
                }
                
            }
            function createDummyNode(docType) {
                vm.Loading = true;
                var currentNodeId = $scope.currentNode.id;
                var data = {
                    DocTypeAlias: docType,
                    ParentNodeId: currentNodeId,
                    Count: $("#" +'dummyContentCount_' + docType).val(),
                    PrefixSequence: $("#" + 'preSequenceTextBox_' + docType).val(),
                    PostfixSequence: $("#" + 'postSequenceTextBox_' + docType).val(),
                    SequenceDigits: $("#" + 'sequenceDigits_' + docType).val(),
                    IsChildrensIncluded: $("#" + 'includeChildCheckBox_' + docType).is(":checked"),
                };
                uCreateDummyNodeResources.process(data).then(function (response) {
                    if (response !== undefined && response.data != undefined) {
                        var url = response.data.data;

                        if (url != undefined && url != "") {
                            vm.Loading = false;
                            navigationService.hideDialog();
                            $window.location.href = url;
                        }
                    }
                });
                vm.Loading = false;
            }
            vm.getAllowedDocTypes();
        });
