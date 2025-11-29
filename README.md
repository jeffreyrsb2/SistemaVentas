# Sistema de Ventas Simplificado (Proyecto de Portafolio)

Este proyecto es una aplicación web full-stack desarrollada con .NET 8 que simula un punto de venta básico, permitiendo la gestión de productos, clientes y la creación de ventas.

El objetivo principal de este proyecto es demostrar la implementación de una **Arquitectura Limpia** robusta y desacoplada en el ecosistema .NET.

## Arquitectura

<p align="center">
  <img src="arquitectura.jpg" width="600" />
</p>

La solución está organizada siguiendo los principios de la Arquitectura Limpia, separando las responsabilidades en capas bien definidas:

-   **`SistemaVentas.Dominio`:** La capa central. Contiene las entidades de negocio (modelos) y las abstracciones (interfaces) de los repositorios. No tiene dependencias externas.
-   **`SistemaVentas.Aplicacion`:** Contiene la lógica de negocio y los casos de uso. Orquesta las operaciones coordinando los repositorios. No sabe nada sobre la base de datos o la API, solo depende de las interfaces del dominio.
-   **`SistemaVentas.Infraestructura`:** La implementación de la capa de acceso a datos. Utiliza **ADO.NET con Stored Procedures** para comunicarse con la base de datos SQL Server, como se solicita en entornos de alto rendimiento.
-   **`SistemaVentas.Api`:** La capa de presentación del backend. Expone los servicios de la capa de aplicación a través de endpoints RESTful, asegurados con **autenticación JWT**.
-   **`SistemaVentas.Web`:** El cliente frontend. Una aplicación **Razor Pages** que consume la API de forma asíncrona utilizando JavaScript (`fetch`), demostrando la separación entre el cliente y el servidor.

## Modelo de Datos (DER)

<p align="center">
  <img src="DER Sistema de Ventas.png" width="600" />
</p>

## Tecnologías Utilizadas

-   **Backend:** C#, .NET 8, ASP.NET Core Web API
-   **Acceso a Datos:** ADO.NET, Stored Procedures, SQL Server
-   **Frontend:** Razor Pages, JavaScript (ES6+), HTML5, CSS3, Bootstrap 5
-   **Seguridad:** JSON Web Tokens (JWT) para autenticación basada en roles.
-   **Arquitectura:** Principios de Clean Architecture y SOLID.

## Cómo Ejecutar Localmente

1.  **Clonar el repositorio.**
2.  **Crear la Base de Datos:** Ejecutar el script SQL `[BD]` sobre una instancia de SQL Server (LocalDB).
3.  **Configurar la Conexión:** Actualizar la `ConnectionString` en el archivo `appsettings.json` del proyecto `SistemaVentas.Api`.
4.  **Ejecutar la Solución:** Abrir el archivo `.sln` en Visual Studio y configurar un inicio múltiple para los proyectos `SistemaVentas.Api` y `SistemaVentas.Web`. Presionar F5.