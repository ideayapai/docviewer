var TableEditable = function () {

    return {

        //main function to initiate the module
        init: function () {
  
            var oTable = $('#edittable').dataTable({
              'bPaginate': false,
              'bInfo': false,
              'bFilter':false,
            });
     
        }

    };

}();