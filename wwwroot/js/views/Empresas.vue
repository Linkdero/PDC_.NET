<template>
    <div>
        <h4>Gestión de Empresas</h4>
        <form class="row g-3 mb-3" @submit.prevent="setNuevaEmpresa">
            <div class="col-md-4">
                <label class="form-label">NIT</label>
                <input v-model="empresa.nit" type="number" class="form-control" placeholder="NIT" required />
            </div>
            <div class="col-md-4">
                <label class="form-label">Razón Social</label>
                <input v-model="empresa.razonSocial" type="text" class="form-control" placeholder="Razón Social"
                    required />
            </div>
            <div class="col-md-4">
                <label class="form-label">Nombre Comercial</label>
                <input v-model="empresa.nombreComercial" type="text" class="form-control" placeholder="Nombre Comercial"
                    required />
            </div>

            <paises-list :evento="evento"></paises-list>
            <departamentos-list :evento="evento"></departamentos-list>
            <municipios-list :evento="evento"></municipios-list>
            <div class="col-md-10">
                <label class="form-label">Correo Electrónico</label>
                <input v-model="empresa.correoElectronico" type="email" class="form-control" placeholder="Correo" required />
            </div>

            <div class="col-md-2">
                <label class="form-label">Teléfono</label>
                <input v-model="empresa.telefono" type="number" class="form-control" placeholder="Teléfono" required />
            </div>
            <div class="col-12">
                <button type="submit" class="btn btn-success">
                    <i class="fas fa-save me-2"></i>Guardar Empresa
                </button>
            </div>
        </form>

        <!-- Tabla de Empresas -->
        <table class="table table-bordered">
            <thead class="table-light">
                <tr>
                    <th>#</th>
                    <th>NIT</th>
                    <th>Razón Social</th>
                    <th>Nombre Comercial</th>
                    <th>Ubicación</th>
                    <th>Teléfono</th>
                    <th>Correo</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="e in empresas" :key="e.id" class="text-center">
                    <td>{{ e.id_empresa }}</td>
                    <td>{{ e.nit }}</td>
                    <td>{{ e.razon_social }}</td>
                    <td>{{ e.nombre_comercial }}</td>
                    <td>{{ e.ubicacion }}</td>
                    <td>{{ e.telefono }}</td>
                    <td>{{ e.correo_electronico }}</td>
                    <td>
                        <button @click="setEliminarEmpresa(e.id_empresa, e.nombre_comercial)"
                            class="btn btn-sm btn-danger">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>
const EventBus = new Vue();
const PaisesList = httpVueLoader("../components/PaisesList.vue");
const DepartamentosList = httpVueLoader("../components/DepartamentosList.vue");
const MunicipiosList = httpVueLoader("../components/MunicipiosList.vue");

module.exports = {
    data: function () {
        return {
            evento: EventBus,
            Toast: null,
            empresa: {
                nit: "",
                razonSocial: "",
                nombreComercial: "",
                idMunicipio: 0,
                telefono: "",
                correoElectronico: "",
            },
            empresas: [],
            pais: "",
            departamento: "",
            municipio: "",
        };
    },
    mounted() {
        this.Toast = Swal.mixin({
            toast: true,
            position: "top-end",
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            didOpen: (toast) => {
                toast.onmouseenter = Swal.stopTimer;
                toast.onmouseleave = Swal.resumeTimer;
            }
        });

        this.getAllEmpresas()
        this.evento.$on("pais-seleccionado", (nuevoValor) => {
            this.pais = nuevoValor;
            this.evento.$emit("activar-departamentos", nuevoValor);
        });

        this.evento.$on("departamento-seleccionado", (nuevoValor) => {
            this.departamento = nuevoValor;
            this.evento.$emit("activar-municipios", nuevoValor);
        });

        this.evento.$on("municipio-seleccionado", (nuevoValor) => {
            this.empresa.idMunicipio = nuevoValor;
        });

    },
    components: {
        "paises-list": PaisesList,
        "departamentos-list": DepartamentosList,
        "municipios-list": MunicipiosList,
    },
    methods: {
        setNuevaEmpresa() {
            if (this.empresa.idMunicipio == 0) {
                this.Toast.fire({
                    icon: "error",
                    title: "Seleccione un municipio",
                }); return;
            }

            Swal.fire({
                title: '¿Desea generar una nueva empresa?',
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Sí, guardar',
                cancelButtonText: 'Cancelar',
            }).then((result) => {
                if (result.value) {
                    axios.post("/api/empresa", this.empresa, {
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    })
                        .then(response => {
                            console.log(response.data);
                            Swal.fire({
                                icon: 'success',
                                title: response.data.msg,
                                showConfirmButton: false,
                                timer: 1500
                            })
                            this.resetearFormulario();
                            this.getAllEmpresas();
                        })
                        .catch(error => {
                            console.error(error);
                        });
                }
            })
        },
        getAllEmpresas() {
            axios.get("api/empresa").then(function (response) {
                this.empresas = response.data;
                console.log("empresas recibidas:", this.empresas);
            }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener empresas:", error);
                });
        },
        resetearFormulario() {
            this.empresa = {
                nit: "",
                razonSocial: "",
                nombreComercial: "",
                idMunicipio: "",
                telefono: "",
                correoElectronico: "",
            };
        },
        setEliminarEmpresa(id, nombre) {
            Swal.fire({
                title: `¿Estás seguro que quieres eliminar la empresa: ${id} - ${nombre}?`,
                text: "Esta acción no se puede deshacer.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    axios.delete(`api/empresa/${id}`)
                        .then(response => {
                            Swal.fire({
                                icon: 'success',
                                title: response.data.msg,
                                showConfirmButton: false,
                                timer: 1500
                            });
                            this.getAllEmpresas();
                        })
                        .catch(error => {
                            console.error('Error al eliminar empresa:', error);
                            Swal.fire({
                                icon: 'error',
                                title: 'Error al eliminar',
                                text: 'Intenta nuevamente más tarde',
                            });
                        });
                }
            });
        }
    }

};
</script>