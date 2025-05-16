<template>
    <div class="col">
        <label for="departamentos" class="font-weight-semibold">Listado de departamentos*:</label>
        <select class="form-control form-control-sm w-100" id="departamentos" required>
            <option disabled selected>SELECCIONE UN DEPARTAMENTO</option>
            <option v-for="d in departamentos" :value="d.id_departamento" :key="d.id_departamento">
                {{ d.departamento }}
            </option>
        </select>
    </div>
</template>

<script>
module.exports = {
    props: ["evento", "modal"],
    data: function () {
        return {
            departamentos: [],
        };
    },

    mounted() {
        this.evento.$on("activar-departamentos", (nuevoValor) => {
            this.getAllDepartamentos(nuevoValor);
        });
    },

    methods: {
        getAllDepartamentos(pais) {
            axios.get("api/departamento",
                {
                    params: {
                        pais: pais,
                    },
                }).then(function (response) {
                    this.departamentos = response.data;
                    console.log("departamentos recibidos:", this.departamentos);

                    setTimeout(() => {
                        $('#departamentos').select2({
                            placeholder: 'departamentos',
                            allowClear: true,
                            width: '100%',
                            create: true,
                            sortField: 'text',
                        });

                        $('#departamentos').on("change", (event) => {
                            const selectedId = $(event.target).val();
                            this.evento.$emit("departamento-seleccionado", selectedId);
                        });
                    }, 100);

                }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener departamentos:", error);
                });
        },
    },
};
</script>