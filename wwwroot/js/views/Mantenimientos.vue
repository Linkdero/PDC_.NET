<template>
    <div>
        <h4>Mantenimientos</h4>
        <div class="row">
            <div class="col-md-4">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <i class="fas fa-globe me-2"></i>Países
                    </div>
                    <div class="card-body">
                        <form @submit.prevent="setNuevoPais" class="mb-3">
                            <div class="input-group">
                                <input v-model="nombreNuevoPais" type="text" class="form-control"
                                    placeholder="Nuevo país" required>
                                <button class="btn btn-success" type="submit">
                                    <i class="fas fa-plus"></i>
                                </button>
                            </div>
                        </form>
                        <ul class="list-group">
                            <li v-for="p in paises" :key="p.id_pais"
                                class="list-group-item d-flex justify-content-between align-items-center">
                                <span class="text-success font-weight-bold" width="200px"
                                    @click="openEditable($event, 1)">
                                    {{ p.pais }}</span>
                                <button @click="setEliminarPais(p.id_pais, p.pais)" class="btn btn-sm btn-danger">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <i class="fas fa-map me-2"></i>Departamentos
                    </div>
                    <div class="card-body">
                        <form @submit.prevent="setNuevoDepartamento" class="mb-3">
                            <paises-list :evento="evento" :key="key"></paises-list>
                            <div class="input-group">
                                <input v-model="nombreNuevoDepartamento" type="text" class="form-control"
                                    placeholder="Nuevo departamento" required>
                                <button class="btn btn-success" type="submit">
                                    <i class="fas fa-plus"></i>
                                </button>
                            </div>
                        </form>
                        <ul class="list-group">
                            <li v-for="d in departamentos" :key="d.id_departamento"
                                class="list-group-item d-flex justify-content-between align-items-center">
                                {{ d.departamento }}
                                <button @click="setEliminarDepartamento(d.id_departamento, d.departamento)"
                                    class="btn btn-sm btn-danger">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>

            <div class="col-md-4">
                <div class="card">
                    <div class="card-header bg-primary text-white">
                        <i class="fas fa-map-marked me-2"></i>Municipios
                    </div>
                    <div class="card-body">
                        <form @submit.prevent="setNuevoMunicipio" class="mb-3">
                            <departamentos-list :evento="evento" :key="key"></departamentos-list>
                            <div class="input-group">
                                <input v-model="nombreNuevoMunicipio" type="text" class="form-control"
                                    placeholder="Nuevo municipio" required>
                                <button class="btn btn-success" type="submit">
                                    <i class="fas fa-plus"></i>
                                </button>
                            </div>
                        </form>
                        <ul class="list-group">
                            <li v-for="m in municipios" :key="m.id_municipio"
                                class="list-group-item d-flex justify-content-between align-items-center">
                                {{ m.municipio }}
                                <button @click="setEliminarMunicipio(m.id_municipio, m.municipio)"
                                    class="btn btn-sm btn-danger">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
const EventBus = new Vue();
const PaisesList = httpVueLoader("../components/PaisesList.vue");
const DepartamentosList = httpVueLoader("../components/DepartamentosList.vue");

module.exports = {
    data() {
        return {
            evento: EventBus,
            key: 0,
            paises: [],
            departamentos: [],
            municipios: [],
            paisSeleccionado: "",
            departamentoSeleccionado: "",
            nombreNuevoPais: "",
            nombreNuevoDepartamento: "",
            nombreNuevoMunicipio: "",
            departamentoPais: "",
            municipioDepartamento: "",
        };
    },
    mounted() {
        this.getAllPaises();
        this.evento.$on("pais-seleccionado", (nuevoValor) => {
            this.paisSeleccionado = nuevoValor;
            this.evento.$emit("activar-departamentos", nuevoValor);
        });

        this.evento.$on("departamento-seleccionado", (nuevoValor) => {
            this.departamentoSeleccionado = nuevoValor;
            this.evento.$emit("activar-municipios", nuevoValor);
        });
    },
    components: {
        "paises-list": PaisesList,
        "departamentos-list": DepartamentosList,
    },
    watch: {
        paisSeleccionado(n) {
            this.getAllDepartamentos();
        },
        departamentoSeleccionado(n) {
            this.getAllMunicipios();
        },
    },
    methods: {
        getAllPaises() {
            axios.get("api/paises").then(function (response) {
                this.paises = response.data;
                console.log("paises recibidos:", this.paises);
            }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener paises:", error);
                });
        },
        getAllDepartamentos() {
            axios.get("api/departamento",
                {
                    params: {
                        pais: this.paisSeleccionado,
                    },
                }).then(function (response) {
                    this.departamentos = response.data;
                    console.log("departamentos recibidos:", this.departamentos);
                }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener departamentos:", error);
                });
        },
        getAllMunicipios() {
            axios.get("api/municipio",
                {
                    params: {
                        departamento: this.departamentoSeleccionado,
                    },
                }).then(function (response) {
                    this.municipios = response.data;
                    console.log("municipios recibidos:", this.municipios);
                }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener municipios:", error);
                });
        },
        setEliminarPais(id, nombre) {
            Swal.fire({
                title: `¿Estás seguro que quieres eliminar el pais: ${id} - ${nombre}?`,
                text: "Esta acción no se puede deshacer.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    axios.delete(`api/paises/${id}`)
                        .then(response => {
                            Swal.fire({
                                icon: 'success',
                                title: response.data.msg,
                                showConfirmButton: false,
                                timer: 1500
                            });
                            this.getAllPaises();
                            this.getAllDepartamentos();
                            this.getAllMunicipios();
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
        },
        setEliminarDepartamento(id, nombre) {
            Swal.fire({
                title: `¿Estás seguro que quieres eliminar el departamento: ${id} - ${nombre}?`,
                text: "Esta acción no se puede deshacer.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    axios.delete(`api/departamento/${id}`)
                        .then(response => {
                            Swal.fire({
                                icon: 'success',
                                title: response.data.msg,
                                showConfirmButton: false,
                                timer: 1500
                            });
                            this.getAllPaises();
                            this.getAllDepartamentos();
                            this.getAllMunicipios();
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
        },
        setEliminarMunicipio(id, nombre) {
            Swal.fire({
                title: `¿Estás seguro que quieres eliminar el municipio: ${id} - ${nombre}?`,
                text: "Esta acción no se puede deshacer.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    axios.delete(`api/municipio/${id}`)
                        .then(response => {
                            Swal.fire({
                                icon: 'success',
                                title: response.data.msg,
                                showConfirmButton: false,
                                timer: 1500
                            });
                            this.getAllPaises();
                            this.getAllDepartamentos();
                            this.getAllMunicipios();
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
        },
        setNuevoPais() {
            if (!this.nombreNuevoPais.trim()) return;

            Swal.fire({
                title: `¿Estás seguro que quieres agregar el país: ${this.nombreNuevoPais}?`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, agregar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    axios.post('api/paises', { nombreNuevoPais: this.nombreNuevoPais }, {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }
                    }).then(response => {
                        Swal.fire({
                            icon: 'success',
                            title: 'País agregado',
                            text: response.data.msg,
                            timer: 1500,
                            showConfirmButton: false
                        });
                        this.nombreNuevoPais = "";
                        this.key++;
                        this.getAllPaises();
                        this.getAllDepartamentos();
                        this.getAllMunicipios();
                    }).catch(error => {
                        console.error('Error al agregar país:', error);
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo agregar el país.',
                        });
                    });
                }
            });
        },
        setNuevoDepartamento() {
            if (!this.nombreNuevoDepartamento.trim() || !this.paisSeleccionado) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Atención',
                    text: 'Debe seleccionar un país y escribir un nombre de departamento.',
                });
                return;
            }

            Swal.fire({
                title: `¿Agregar el departamento: ${this.nombreNuevoDepartamento.trim()}?`,
                text: `País seleccionado: ${this.paisSeleccionado}`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, agregar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    axios.post('api/departamento', {
                        nombreNuevoDepartamento: this.nombreNuevoDepartamento,
                        idPais: this.paisSeleccionado
                    }, {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }
                    }).then(response => {
                        Swal.fire({
                            icon: 'success',
                            title: 'Departamento agregado',
                            text: response.data.msg,
                            timer: 1500,
                            showConfirmButton: false
                        });
                        this.nombreNuevoDepartamento = "";
                        this.key++;
                        this.getAllPaises();
                        this.getAllDepartamentos();
                        this.getAllMunicipios();
                    }).catch(error => {
                        console.error('Error al agregar departamento:', error);
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo agregar el departamento.',
                        });
                    });
                }
            });
        },
        setNuevoMunicipio() {
            if (!this.nombreNuevoMunicipio.trim() || !this.departamentoSeleccionado) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Atención',
                    text: 'Debe seleccionar un departamento y escribir un nombre de municipio.',
                });
                return;
            }

            Swal.fire({
                title: `¿Agregar el municipio: ${this.nombreNuevoMunicipio.trim()}?`,
                text: `Departamento seleccionado: ${this.departamentoSeleccionado}`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, agregar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    axios.post('api/municipio', {
                        nombreNuevoMunicipio: this.nombreNuevoMunicipio,
                        idDepartamento: this.departamentoSeleccionado
                    }, {
                        headers: {
                            'Content-Type': 'application/x-www-form-urlencoded'
                        }
                    }).then(response => {
                        Swal.fire({
                            icon: 'success',
                            title: 'Municipio agregado',
                            text: response.data.msg,
                            timer: 1500,
                            showConfirmButton: false
                        });
                        this.nombreNuevoMunicipio = "";
                        this.key++;
                        this.getAllPaises();
                        this.getAllDepartamentos();
                        this.getAllMunicipios();
                    }).catch(error => {
                        console.error('Error al agregar municipio:', error);
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo agregar el municipio.',
                        });
                    });
                }
            });
        },

    },

};
</script>