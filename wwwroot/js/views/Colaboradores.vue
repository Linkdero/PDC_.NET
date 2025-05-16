<template>
    <div>
        <h4>Gestión de Colaboradores</h4>
        <form class="row g-3 mb-3" @submit.prevent="setNuevoColaborador">
            <div class="col-md-4">
                <label class="form-label">Nombre</label>
                <input v-model="colaborador.nombre" type="text" class="form-control" placeholder="Nombre" required />
            </div>
            <div class="col-md-4">
                <label class="form-label">Apellido</label>
                <input v-model="colaborador.apellido" type="text" class="form-control" placeholder="Apellido"
                    required />
            </div>
            <div class="col-md-4">
                <label class="form-label">Edad</label>
                <input v-model="colaborador.edad" type="number" class="form-control" placeholder="Edad" required />
            </div>
            <div class="col-md-4">
                <label class="form-label">Teléfono</label>
                <input v-model="colaborador.telefono" type="text" class="form-control" placeholder="Teléfono"
                    required />
            </div>
            <div class="col-md-4">
                <label class="form-label">Correo Electrónico</label>
                <input v-model="colaborador.correo" type="email" class="form-control" placeholder="Correo" required />
            </div>

            <empresas-list :evento="evento"></empresas-list>

            <div class="col-md-6">
                <label class="form-label">Empresas Seleccionadas</label>
                <ul class="list-group">
                    <li class="list-group-item d-flex justify-content-between align-items-center"
                        v-for="(e, index) in colaborador.empresas" :key="e.id">
                        {{ e.nombre }}
                        <button type="button" class="btn btn-sm btn-danger" @click="quitarEmpresa(index)">
                            <i class="fas fa-times"></i>
                        </button>
                    </li>
                </ul>
            </div>

            <div class="col-12">
                <button type="submit" class="btn btn-success">
                    <i class="fas fa-save me-2"></i>Guardar Colaborador
                </button>
            </div>
        </form>

        <!-- Tabla de Colaboradores -->
        <table class="table table-bordered">
            <thead class="table-light">
                <tr>
                    <th>#</th>
                    <th>Nombre</th>
                    <th>Edad</th>
                    <th>Teléfono</th>
                    <th>Correo</th>
                    <th>Empresas</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="c in colaboradores" :key="c.id_colaborador">
                    <td>{{ c.id_colaborador }}</td>
                    <td>{{ c.nombre }} {{ c.apellido }}</td>
                    <td>{{ c.edad }}</td>
                    <td>{{ c.telefono }}</td>
                    <td>{{ c.correo }}</td>
                    <td>
                        <ul>
                            <li v-for="(empresa, index) in c.nombres_comerciales.split(',')" :key="index">
                                {{ empresa.trim() }}
                            </li>
                        </ul>
                    </td>
                    <td>
                        <button @click="setEliminarColaborador(c.id_colaborador, c.nombre)"
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
const EmpresasList = httpVueLoader("../components/EmpresasList.vue");
const EventBus = new Vue();

module.exports = {
    data() {
        return {
            colaborador: {
                nombre: "",
                apellido: "",
                edad: "",
                telefono: "",
                correo: "",
                empresas: [],
            },
            colaboradores: [],
            evento: EventBus,
        };
    },

    mounted() {
        this.getAllColaboradores();
        this.evento.$on("empresa-seleccionada", (id, nombre) => {
            const existe = this.colaborador.empresas.some(e => e.id === id);
            if (!existe) {
                this.colaborador.empresas.push({
                    idEmpresa: id,
                    nombre: nombre,
                });
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Empresa ya seleccionada',
                    timer: 1500,
                    showConfirmButton: false
                });
            }
        });
    },

    components: {
        "empresas-list": EmpresasList,
    },

    methods: {
        quitarEmpresa(index) {
            this.colaborador.empresas.splice(index, 1);
        },
        setNuevoColaborador() {
            Swal.fire({
                title: '¿Desea guardar el colaborador?',
                icon: 'question',
                showCancelButton: true,
                confirmButtonText: 'Sí, guardar',
                cancelButtonText: 'Cancelar',
            }).then((result) => {
                if (result.value) {
                    axios.post("api/colaborador", this.colaborador)
                        .then(response => {
                            Swal.fire({
                                icon: 'success',
                                title: response.data.msg,
                                showConfirmButton: false,
                                timer: 1500
                            });
                            this.getAllColaboradores();
                            this.resetFormulario();
                        })
                        .catch(error => {
                            console.error(error);
                        });
                }
            });
        },
        getAllColaboradores() {
            axios.get("api/colaborador").then(function (response) {
                    this.colaboradores = response.data;
                    console.log("Colaboradores recibidos:", this.colaboradores);
                }.bind(this))
                .catch(function (error) {
                    console.error("Error al obtener colaboradores:", error);
                });
        },
        resetFormulario() {
            this.colaborador = {
                nombre: "",
                apellido: "",
                edad: "",
                telefono: "",
                correo: "",
                empresas: [],
            };
        },

        setEliminarColaborador(id, nombre) {
            Swal.fire({
                title: `¿Estás seguro que quieres eliminar al colaborador: ${id} - ${nombre}?`,
                text: "Esta acción no se puede deshacer.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    axios.delete(`api/colaborador/${id}`)
                        .then(response => {
                            Swal.fire({
                                icon: 'success',
                                title: response.data.msg,
                                showConfirmButton: false,
                                timer: 1500
                            });
                            this.getAllColaboradores();
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
    },
};
</script>
