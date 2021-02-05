

$(document).ready(function () {

    //$("#clienteCola").on("click", function (e) {
    //    e.stopPropagation();
    //    e.preventDefault();

    //    //alert("");

    //});
    $("#txtGuardar").on("click", function (e) {
        e.stopPropagation();
        e.preventDefault();

        
        GuardarRegistro();

    });

    function GuardarRegistro() {
        let cliente = {
            idCliente: $('#txtIdCliente').val(),
            nombreCliente: $('#txtNombreCliente').val()
        };

        var urlInsertar = document.baseURI + 'api/Home/InsertClienteFrontEnd';
        
        $.ajax({
            method: 'POST',
            url: urlInsertar,
            data: {
                cliente: cliente
               
            },
            dataType: 'json',
            async: false
        }).done(function (data) {

           
        }).fail(function () {
            //muestraMensaje(nombreModulo, "error", "Error, comuníquese con el administrador.");
        });


    }

   
    function checkTime() {
        
        var urlInsertar = document.baseURI + 'api/Home/checkTime';

        $.ajax({
            method: 'POST',
            url: urlInsertar,
            dataType: 'json',
            async: true,
            success: function (data) {
                
            }
        });

    }
    setInterval(checkTime, 10000);
   

});