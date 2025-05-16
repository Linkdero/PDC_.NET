<template>
    <div class="col">
        <label for="empresas" class="font-weight-semibold">Listado de Empresas*:</label>
        <select class="form-control form-control-sm w-100" id="empresas" required>
            <option disabled selected>SELECCIONE UNA EMPRESA</option>
            <option v-for="e in empresas" :value="e.id_empresa" :key="e.id_empresa">
                {{ e.nit }} - {{ e.razon_social }} - {{ e.nombre_comercial }}
            </option>
        </select>
    </div>
</template>

<script>
module.exports = {
    props: ["evento", "modal"],
    data: function () {
        return {
            empresas: [],
        };
    },

    mounted() {
        this.getAllempresas();
    },

    methods: {
        getAllempresas() {
            axios.get("api/empresa").then(function (response) {
                    this.empresas = response.data;
                    console.log("empresas recibidas:", this.empresas);

                    setTimeout(() => {
                        $('#empresas').select2({
                            placeholder: 'empresas',
                            allowClear: true,
                            width: '100%',
                            create: true,
                            sortField: 'text',
                        });

                        $('#empresas').on("change", (event) => {
                            const selectedId = $(event.target).val();
                            const nombreEmpresa = $("#empresas option:selected").text();
                            this.evento.$emit("empresa-seleccionada", selectedId, nombreEmpresa);
                        });
                    }, 100);

                }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener empresas:", error);
                });
        },
    },
};
</script>