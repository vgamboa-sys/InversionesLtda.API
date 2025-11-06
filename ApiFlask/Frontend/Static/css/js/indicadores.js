$(document).ready(function () {
    let exchangeRates = {}; // Guardaremos las tasas de cambio aquí

    // Función para consumir la API y obtener los valores
    function fetchExchangeRates() {
        $.getJSON('https://mindicador.cl/api', function (data) {
            // Guardamos las tasas de cambio relevantes
            exchangeRates = {
                uf: data.uf.valor,
                usd: data.dolar.valor,
                eur: data.euro.valor,
                clp: 1 // Peso chileno (CLP) es la moneda base
            };

            // Mostrar los valores del Dólar y Euro
            $('#usdValue').html(`Dólar (USD): $${data.dolar.valor}`);
            $('#eurValue').html(`Euro (EUR): €${data.euro.valor}`);

            // Llenar las monedas de destino al cargar la API
            updateToCurrencies($('#fromCurrency').val());
        }).fail(function () {
            console.log('Error al consumir la API!');
        });
    }

    // Función para actualizar las monedas de destino según la moneda de origen seleccionada
    function updateToCurrencies(fromCurrency) {
        let toCurrenciesHtml = '<div class="row">';  // Añadir la clase row aquí para asegurarse de que esté en una sola fila.

        if (fromCurrency === 'clp') {
            toCurrenciesHtml += `
                <div class="col-12 col-md-6">
                    <p id="usdResult">USD: <span id="usdAmount" style="color: #f8b400;"></span></p>
                </div>
                <div class="col-12 col-md-6">
                    <p id="eurResult">EUR: <span id="eurAmount" style="color: #f8b400;"></span></p>
                </div>
            `;
        } else if (fromCurrency === 'usd') {
            toCurrenciesHtml += `
                <div class="col-12 col-md-6">
                    <p id="clpResult">CLP: <span id="clpAmount" style="color: #f8b400;"></span></p>
                </div>
                <div class="col-12 col-md-6">
                    <p id="eurResult">EUR: <span id="eurAmount" style="color: #f8b400;"></span></p>
                </div>
            `;
        } else if (fromCurrency === 'eur') {
            toCurrenciesHtml += `
                <div class="col-12 col-md-6">
                    <p id="clpResult">CLP: <span id="clpAmount" style="color: #f8b400;"></span></p>
                </div>
                <div class="col-12 col-md-6">
                    <p id="usdResult">USD: <span id="usdAmount" style="color: #f8b400;"></span></p>
                </div>
            `;
        }

        toCurrenciesHtml += '</div>';  // Cerrar la fila
        $('#toCurrencies').html(toCurrenciesHtml);
    }

    // Función para realizar la conversión de moneda
    $('#convertButton').click(function (event) {
        event.preventDefault(); // Evita el comportamiento predeterminado
        event.stopPropagation();
        const amount = parseFloat($('#amount').val());
        const fromCurrency = $('#fromCurrency').val();

        // Validar que la cantidad no sea negativa ni vacía
        if (isNaN(amount) || amount <= 0) {
            alert("Por favor ingresa una cantidad válida.");
            return;
        }

        // Realizamos la conversión según la moneda de origen seleccionada
        if (fromCurrency === 'clp') {
            const usdAmount = amount / exchangeRates.usd;
            const eurAmount = amount / exchangeRates.eur;
            $('#usdAmount').html(usdAmount.toFixed(3));
            $('#eurAmount').html(eurAmount.toFixed(3));
        } else if (fromCurrency === 'usd') {
            const clpAmount = amount * exchangeRates.usd;
            const eurAmount = amount * exchangeRates.usd / exchangeRates.eur;
            $('#clpAmount').html(clpAmount.toFixed(0));
            $('#eurAmount').html(eurAmount.toFixed(2));
        } else if (fromCurrency === 'eur') {
            const clpAmount = amount * exchangeRates.eur;
            const usdAmount = amount * exchangeRates.eur / exchangeRates.usd;
            $('#clpAmount').html(clpAmount.toFixed(0));
            $('#usdAmount').html(usdAmount.toFixed(3));
        }
    });

    // Obtener los valores de la API al cargar la página
    fetchExchangeRates();

    // Actualizar las monedas de destino cuando se cambie la moneda de origen
    $('#fromCurrency').change(function () {
        updateToCurrencies($(this).val());
    });
});