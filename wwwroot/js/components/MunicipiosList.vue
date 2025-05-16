<template>
    <div class="col">
        <label for="municipios" class="font-weight-semibold">Listado de Municipios*:</label>
        <select class="form-control form-control-sm w-100" id="municipios" required>
            <option disabled selected>SELECCIONE UN MUNICIPIO</option>
            <option v-for="m in municipios" :value="m.id_municipio" :key="m.id_municipio">
                {{ m.municipio }}
            </option>
        </select>
    </div>
</template>

<script>
module.exports = {
    props: ["evento", "modal"],
    data: function () {
        return {
            municipios: [],
        };
    },

    mounted() {
        this.evento.$on("activar-municipios", (nuevoValor) => {
            this.getAllMunicipios(nuevoValor);
        });
    },

    methods: {
        getAllMunicipios(departamento) {
            axios.get("api/municipio",
                {
                    params: {
                        departamento: departamento,
                    },
                }).then(function (response) {
                    this.municipios = response.data;
                    console.log("municipios recibidos:", this.municipios);

                    setTimeout(() => {
                        $('#municipios').select2({
                            placeholder: 'municipios',
                            allowClear: true,
                            width: '100%',
                            create: true,
                            sortField: 'text',
                        });

                        $('#municipios').on("change", (event) => {
                            const selectedId = $(event.target).val();
                            this.evento.$emit("municipio-seleccionado", selectedId);
                        });
                    }, 100);
                }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener municipios:", error);
                });
        },
    },
};
</script>