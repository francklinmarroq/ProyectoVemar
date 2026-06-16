# Diagrama Entidad-Relación - Base de Datos Vemar

```mermaid
erDiagram

    %% ==================== TABLAS DE REFERENCIA ====================

    CategoriasProyectos {
        int Id PK
        string Nombre
        int CantidadLotes
    }

    Zonificaciones {
        int Id PK
        string Zonificacion
    }

    EstadosTramites {
        int Id PK
        string Estado
    }

    TiposTramites {
        int Id PK
        string Nombre
    }

    TiposMovimientos {
        int Id PK
        string Tipo
    }

    TiposUsuarios {
        int Id PK
        string Tipo
    }

    %% ==================== ENTIDADES PRINCIPALES ====================

    Clientes {
        int Id PK
        string Rtn
        string Nombre
        string Direccion
        string Representante
        string DniRepresentante
        string RtnRepresentante
        string Telefono
        string EmailRepresentante
        string EmailCorporativo
    }

    Colaboradores {
        int Id PK
        string Dni
        string Nombre
        string Telefono
        datetime FechaNacimiento
        string Cargo
        string Domicilio
        string Email
    }

    Contratistas {
        int Id PK
        string Nombre
        string Telefono
    }

    Usuarios {
        int Id PK
        string Usuario
        string HashContrasena
        int TipoUsuarioId FK
    }

    %% ==================== ENTIDADES DE PROYECTO ====================

    Proyectos {
        int Id PK
        string Nombre
        int ClienteId FK
        string Ubicacion
        int ZonificacionId FK
        decimal Area
        int CategoriaProyectoId FK
        string Matricula
        string ClaveSure
    }

    Contratos {
        int Id PK
        int ContratistaId FK
        decimal Valor
        int ProyectoId FK
    }

    Tramites {
        int Id PK
        int TipoTramiteId FK
        int ProyectoId FK
        int EstadoTramiteId FK
        string Descripcion
    }

    Avances {
        int Id PK
        int ProyectoId FK
        string Descripcion
        datetime Fecha
    }

    GastosProyectos {
        int Id PK
        int ProyectoId FK
        string Descripcion
        decimal Cantidad
        decimal CostoUnitario
        bit PendienteDePago
    }

    %% ==================== ASIGNACIONES ====================

    Asignaciones {
        int Id PK
        int ColaboradorId FK
        int ProyectoId FK
        datetime FechaAsignacion
        datetime FechaFinalizacion
        string Observaciones
        int ClienteId FK
    }

    %% ==================== ENTIDADES DE REMEDIDA ====================

    Remedidas {
        int Id PK
        int ClienteId FK "NOT NULL"
        string Representante
        string Ubicacion
        string ClaveSure
        string Matricula
        string Cam
        string Objeto
        datetime Fecha
        decimal Precio
        bit ExpedienteEntregado
    }

    Movimientos {
        int Id PK
        int RemedidaId FK
        int TipoMovimientoId FK
        datetime Fecha
        string Descripcion
    }

    CobrosRemedidas {
        int Id PK
        int RemedidaId FK
        decimal Cantidad
    }

    GastosRemedidas {
        int Id PK
        int RemedidaId FK
        string Descripcion
        decimal Cantidad
        decimal CostoUnitario
        bit PendienteDePago
    }

    %% ==================== PAGOS ====================

    PagosContratos {
        int Id PK
        int ContratoId FK
        decimal Valor
        string Descripcion
    }

    %% ==================== RELACIONES ====================

    %% Referencias
    Usuarios }o--|| TiposUsuarios : "pertenecen a"
    Proyectos }o--|| Clientes : "pertenecen a"
    Proyectos }o--|| Zonificaciones : "tienen"
    Proyectos }o--|| CategoriasProyectos : "clasificados en"

    %% Proyecto -> Hijos
    Proyectos ||--o{ Contratos : "tienen"
    Proyectos ||--o{ Tramites : "tienen"
    Proyectos ||--o{ Avances : "tienen"
    Proyectos ||--o{ GastosProyectos : "tienen"
    Proyectos ||--o{ Asignaciones : "tienen"

    %% Contrato -> Pago
    Contratos ||--o{ PagosContratos : "tienen"

    %% Contrato -> Contratista
    Contratistas ||--o{ Contratos : "realizan"

    %% Tramite -> Tipos y Estados
    TiposTramites ||--o{ Tramites : "clasifican"
    EstadosTramites ||--o{ Tramites : "definen estado"

    %% Asignaciones
    Colaboradores ||--o{ Asignaciones : "asignados en"
    Clientes ||--o{ Asignaciones : "asignados en"

    %% Remedidas
    Clientes ||--o{ Remedidas : "realizan"
    Remedidas ||--o{ Movimientos : "tienen"
    Remedidas ||--o{ CobrosRemedidas : "tienen cobros"
    Remedidas ||--o{ GastosRemedidas : "tienen gastos"

    %% Movimiento -> Tipo
    TiposMovimientos ||--o{ Movimientos : "clasifican"
```

## Resumen de Relaciones

| Tabla Padre | Tabla Hijo | Cardinalidad | FK |
|---|---|---|---|
| **Clientes** | Proyectos | 1:N | `ClienteId` |
| **Clientes** | Asignaciones | 1:N | `ClienteId` |
| **Clientes** | Remedidas | 1:N | `ClienteId` (NOT NULL) |
| **Zonificaciones** | Proyectos | 1:N | `ZonificacionId` |
| **CategoriasProyectos** | Proyectos | 1:N | `CategoriaProyectoId` |
| **Contratistas** | Contratos | 1:N | `ContratistaId` |
| **Proyectos** | Contratos | 1:N | `ProyectoId` |
| **Proyectos** | Tramites | 1:N | `ProyectoId` |
| **Proyectos** | Avances | 1:N | `ProyectoId` |
| **Proyectos** | GastosProyectos | 1:N | `ProyectoId` |
| **Proyectos** | Asignaciones | 1:N | `ProyectoId` |
| **Contratos** | PagosContratos | 1:N | `ContratoId` |
| **TiposTramites** | Tramites | 1:N | `TipoTramiteId` |
| **EstadosTramites** | Tramites | 1:N | `EstadoTramiteId` |
| **Colaboradores** | Asignaciones | 1:N | `ColaboradorId` |
| **Remedidas** | Movimientos | 1:N | `RemedidaId` |
| **Remedidas** | CobrosRemedidas | 1:N | `RemedidaId` |
| **Remedidas** | GastosRemedidas | 1:N | `RemedidaId` |
| **TiposMovimientos** | Movimientos | 1:N | `TipoMovimientoId` |
| **TiposUsuarios** | Usuarios | 1:N | `TipoUsuarioId` |

## Notas

- Todas las tablas usan `int Id` como PK con auto-incremento
- Todas las FK son **nullable** excepto `Remedidas.ClienteId`
- No existen relaciones many-to-many
- No hay constraints de unicidad definidos
