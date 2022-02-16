// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var oTable = $('#iProspecto').DataTable({
            scrollY: "240px",
            scrollX: true,
            scrollCollapse: true,
            fixedColumns: true,
            scrollCollapse: true,
            scroller: true,
            searching: true,
            paging: true,
            pageLength: 5,
            responsive: true,
        columnDefs: [
            { className: 'text-center', width: 50, targets: [0,] },
            { className: 'text-right', width: 200, targets: [1,2,3] },
            { className: 'text-center', width: 80, targets: 4 }
        ],
        fixedColumns: {
            leftColumns: 1
        },
        lengthMenu: [[5, 10, 15, 20, -1], [5, 10, 15, 20, "All"]]
    });
    $('.dataTables_length').addClass('bs-select');
});
//abre cualquier modal
$(document).ready(function () {
    $('a.modal-link').click(function (event) {
        event.preventDefault();

        var modalnom = $(this).attr("data-target"); //Nombre Del Modal
        var id = $(this).data('assigned-id'); //obtiene el id del registro actual
        var url = $(this).data('url') + id; //action,modelo para la vista parcial
        var title = $(this).data('title');   //Titulo

        $("#titulo").html(title);
        $('#Contenido').load(url);
        $(modalnom).modal('show');
    });

    $('body').on('hidden.bs.modal', function () {
        $(this).find('.modal-body').empty();
    });
});
//busca colonia
$(document).ready(function () {
    $("#SCol1").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/CProspecto/AutoComplete",
                type: "POST",
                dataType: "json",
                data: { Prefix: request.term },
                success: function (data) {
                    //event.preventDefault();
                    response($.map(data, function (item) {
                        return { label: item.nombre, value: item.nombre, id: item.id };
                    }))
                }
            })
        },
        select: function (event, ui) {
            document.getElementById('Colonia').value = llenarCadena(ui.item.id,5,"0");
        },
        minLength: 0,
        delay: 500,
    });
});

$(document).on('change', "#SCol1", function (evento) {
    var idcalle = '';
    var lopc = 0;
    var lcol = "";

    lopc = 1;
    lcol = document.getElementById('Colonia').value
    idcalle = 'solCalle'

    CargarCallexColoniaPP(lopc, lcol, idcalle);
});

function CargarCallexColoniaPP(lopc, lcol, idnom) {
    var calleSelect = $('#' + idnom);
    calleSelect.empty();

    $.getJSON('/CProspecto/Llenar_Calle', { col: lcol, opc: lopc }, function (result) {

        var Select2 = $('#' + idnom);
        Select2.empty();
        if (result.length == null || result.length == 0) {
            // si no encuentra calles de esa colonia la desactiva
            //Select2.attr('disabled', 'disabled');
            return;
        }

        //Select2.removeAttr('disabled');
        Select2.empty().append($('<option></option>').val('').text('- Seleccione Valor -'));
        $(result).each(function () {
            $(document.createElement('option'))
                .attr('value', this.value)
                .text(this.text)
                .appendTo(calleSelect);
        });
    });
};

function AgregarCalle(action) {
    var valorcol = document.getElementById('Colonia').value
    var nomOc = "";

    nomOc = $("#newCalleSol").val();
    if (nomOc.length > 0 && valorcol.length > 0) {

        $("#loading").fadeIn()
        $.ajax({
            url: action,
            type: "POST",
            data: { lcol: valorcol, lcalle: nomOc },
            error: function (jqXHR, textStatus, errorThrown) {
                ErroresAjax(jqXHR, textStatus);// Otro manejador error
                ev.preventDefault();
                return false;
            }
        }).done(function (response) {
            if (response != null) {
                if (response == "SAVE") {
                    CargarCallexColoniaPP(1, valorcol, 'solCalle');
                    $("#solCalle").focus();
                    return false;
                }
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            ErroresAjax(jqXHR, textStatus);
            $("#loading").fadeOut();
            alertify.alert('Información', 'Error!', function () { }).setting({ 'modal': false, 'closable': false });
            return false;
        }).always(function (jqXHR, textStatus, errorThrown) {
            $("#loading").fadeOut();
        });
    }
    else {
        alertify.alert('Información', 'Colonia o Calle Vacia!', function () { }).setting({ 'modal': false, 'closable': false });
    }
}

function GuardarComRechazo() {
    var xID = $("#IdRechazo").val();
    var xcom = $("#ComRechazo").val()

    if (xcom.trim().length > 0) {
        $("#loading").fadeIn()
        $.ajax({
            url: "/CProspecto/GuardarComR_Accion",
            type: "POST",
            data: { ID: xID, comentario: xcom },
            error: function (jqXHR, textStatus, errorThrown) {
                ErroresAjax(jqXHR, textStatus);// Otro manejador error
                ev.preventDefault();
                return false;
            }
        }).done(function (data) {
            if (data == "SAVE") {
                $('#btnAutoriza').addClass("disabled").prop("disabled", true)
                $('#btnRechaza').addClass("disabled").prop("disabled", true)
                alertify.set('notifier', 'position', 'top-center');
                alertify.success('Rechazo: Comentario Guardado');
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            ErroresAjax(jqXHR, textStatus);
            $("#loading").fadeOut();
            alertify.alert('Información', 'Error!', function () { }).setting({ 'modal': false, 'closable': false });
            return false;
        }).always(function (jqXHR, textStatus, errorThrown) {
            $("#loading").fadeOut();
        });
    }
    else {
        alertify.alert()
            .setting({
                title: 'Información',
                label: 'Ok',
                message: 'Comentario NO Puede Estar Vacio!',
                onok: function () {
                    $('#ComRechazo').focus();
                },
                modal: false,
                closable: false
            }).show();
    }
}
//------------------------------------------------
//Trabajo con archivos
$(function () {

    // Este código adjuntará el evento `fileselect` a todas las entradas de archivo en la página
    $(document).on('change', ':file', function () {
        var siZefile = formatMoney(this.files[0].size / 1024)
        var tma = siZefile <= 99.99 ? siZefile + " KB" : siZefile + " MB";
        $("#sizeFile").val(tma);

        var input = $(this),
            numFiles = input.get(0).files ? input.get(0).files.length : 1,
            label = input.val().replace(/\\/g, '/').replace(/.*\//, '');
        input.trigger('fileselect', [numFiles, label]);
    });


    $(document).ready(function () {
        //bel siguiente código se ejecuta al cambiar la entrada del archivo y agregar el nombre en el control de texto
        $(':file').on('fileselect', function (event, numFiles, label) {

            var input = $(this).parents('.input-group').find(':text'),
                log = numFiles > 1 ? numFiles + ' files selected' : label;

            if (input.length) {
                input.val(log);
            } else {
                if (log) alert(log);
            }

        });
    });

});

var container = $('#fileUploadContainer');
var template = $('#template');
$('#btnAdd').click(function () {
    var clone = template.clone();
    container.append(clone.html());
    $(this).text('Add another file');
});
container.on('change', '.file', function () {
    var siZefile = formatMoney(this.files[0].size / 1024)
    var fileSize = siZefile <= 99.99 ? siZefile + " KB" : siZefile + " MB";

    //var file = $(this).get(0).files[0];
    //var fileSize = 'Size: ' + Math.round(file.size / 1024) + ' KB'
    $(this).parent('.uploadContainer').next('.controls').find('.filesSize').text(fileSize);
});
//------------------------------------------------
//---------------------------------------------
//FORMATEA NUMEROS CON COMAS Y DECIMALES ESPECIFICADOS
//---------------------------------------------
function formatMoney(amount, decimalCount = 2, decimal = ".", thousands = ",") {
    try {
        decimalCount = Math.abs(decimalCount);
        decimalCount = isNaN(decimalCount) ? 2 : decimalCount;

        const negativeSign = amount < 0 ? "-" : "";

        let i = parseInt(amount = Math.abs(Number(amount) || 0).toFixed(decimalCount)).toString();
        let j = (i.length > 3) ? i.length % 3 : 0;

        return negativeSign + (j ? i.substr(0, j) + thousands : '') + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousands) + (decimalCount ? decimal + Math.abs(amount - i).toFixed(decimalCount).slice(2) : "");
    } catch (e) {
        console.log(e)
    }
};
//-----------------------autoriza prospecto
function ProsAut() {
    alertify.confirm("Pregunta","Seguro Que Desea Autorizar Prospecto?",
        function () {
            $("#loading").fadeIn()
            var lid = document.getElementById('IdAut').value
            $.ajax({
                url: '/CProspecto/Autorizado',
                type: "POST",
                dataType: "json",
                data: { id: lid },
                error: function (jqXHR, textStatus, errorThrown) {
                    ErroresAjax(jqXHR, textStatus);// Otro manejador error
                    $("#loading").fadeOut();
                    return false;
                }
            }).done(function (data) {
                if (data.param1 == "Ok") {
                    $('#btnAutoriza').addClass("disabled").prop("disabled", true)
                    $('#btnRechaza').addClass("disabled").prop("disabled", true)
                    alertify.set('notifier', 'position', 'top-center');
                    alertify.success('Prospecto Autorizado');
                }
            }).fail(function (jqXHR, textStatus, errorThrown) {
                ErroresAjax(jqXHR, textStatus);
                $("#loading").fadeOut();
                alertify.alert('Información', 'Error!', function () { }).setting({ 'modal': false, 'closable': false });
                return false;
            }).always(function (jqXHR, textStatus, errorThrown) {
                $("#loading").fadeOut();
            });
        },
        function () {
            alertify.set('notifier', 'position', 'top-center');
            alertify.error('Autorización Rechazada');
        });
}

//---------------------------------------------
//------AGREGA CEROS A LA IZQUIERDA A CADENAS
//---------------------------------------------
function llenarCadena(cadena, longitud, caracter) {
    //Siendo "cadena" el texto a completar
    //"longitud" el largo de la cadena deseada
    //"caracter" el símbolo con el cual se llenarán los espacios faltantes
    return caracter.repeat(longitud - String(cadena).length).concat(cadena);
}

function ErroresAjax(qXHR, txtStatus) {
    if (qXHR.status === 0) {
        alert('Not connect: Verify Network.\n' + qXHR.responseText);

    } else if (qXHR.status == 401) {
        //window.location.replace("/Identity/Account/Login");
        window.location.replace("../Home/TimeoutRedirect");
    } else if (qXHR.status == 404) {
        alert('Requested page not found [404]\n' + qXHR.responseText);

    } else if (qXHR.status == 500) {
        alert('Internal Server Error [500].\n' + qXHR.responseText);

    } else if (txtStatus === 'parsererror') {
        alert('Requested JSON parse failed.\n' + "Falla en Json");

    } else if (txtStatus === 'timeout') {
        alert('Time out error.\n' + "Tiempo Agotado");

    } else if (txtStatus === 'abort') {
        alert('Ajax request aborted.\n' + "La Solicitud Ha Sido Abortada");

    } else {
        alert('Uncaught Error: ' + qXHR.responseText);
    }
}
