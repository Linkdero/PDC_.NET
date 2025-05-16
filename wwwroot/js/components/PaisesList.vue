<template>
    <div class="col">
        <label for="paises" class="font-weight-semibold">Listado de Paises*:</label>
        <select class="form-control form-control-sm w-100" id="paises" required>
            <option disabled selected>SELECCIONE UN PAIS</option>
            <option v-for="p in paises" :value="p.id_pais" :key="p.id_pais">
                {{ p.pais }}
            </option>
        </select>
    </div>
</template>

<script>
module.exports = {
    props: ["evento", "modal"],
    data: function () {
        return {
            paises: [],
        };
    },

    mounted() {
        this.getAllPaises();
    },

    methods: {
        getAllPaises() {
            axios.get("api/paises").then(function (response) {
                    this.paises = response.data;
                    console.log("paises recibidos:", this.paises);

                    setTimeout(() => {
                        $('#paises').select2({
                            placeholder: 'paises',
                            allowClear: true,
                            width: '100%',
                            create: true,
                            sortField: 'text',
                        });

                        $('#paises').on("change", (event) => {
                            const selectedId = $(event.target).val();
                            this.evento.$emit("pais-seleccionado", selectedId);
                        });
                    }, 100);

                }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener paises:", error);
                });
        },
    },
};
</script>