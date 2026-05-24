# Proyecto 1: Herramienta Administrativa de Base de Datos (Database Manager Tool)

`Teoria Base de Datos 2: Proyecto 1
Cristina Sabillón - 22351004`

## Descripción
El sistema de **Database Manager Tool** ayuda a manejar las bases de datos como el 

---

## Tecnologías utilizadas
- **Base de datos:** Microsoft SQL Server  
- **Gestión de BD:** SQL Server Management Studio  
- **Lenguage de Programacion:** C#
- **Frontend:** App

---

## Objetivo
Desarrollar una herramienta administrativa para bases de datos, interactuando directamente con las tablas de sistema (system tables) del SGBD asignado. 

---
## Soporte para objetos de base de datos
• Tablas Vistas
• Procedimientos almacenados
• Funciones
• Secuencias/Generadores
• Disparadores (Triggers)
• Índices
• Usuarios

// MS SQL SERVER no tiene soporte para la metadata de Tablespaces y Paquetes.
---

## Alcance del Sistema
El sistema incluye lo siguiente:

- **Gestión de conexiones y autenticación.** 
- **Soporte para objetos de base de datos.**
- **Operaciones sobre objetos.**  
- **Ejecución de sentencias SQL**
---

**Consideraciones técnicas**
- Proyecto individual.
- Lenguaje de programación a elección del estudiante.
- El proyecto debe ser Web o Desktop, no se permite modo consola.
- Uso obligatorio y explícito de system tables para obtener metadata.
- No se permite el uso de frameworks/librerías tipo SQLAlchemy, Dapper, Entity Framework, Hibernate, etc.
- No se permite el esquema estandarizado information_schema.
- Documentar cualquier limitación o diferencia en el SGBD.

---

## Datos de prueba
- Conexion a Base de Datos realizada el trimestre pasado.  
- Visualización de las tablas, funciones, procedimientos y triggers de la base de datos previa.  

---

## DATOS DE PRUEBA
Para la prueba se utilizo :
- Server: localhost
- Base de datos: sistema_bancario

---

## REFLEXION SOBRE EL PROCESO DEL PROYECTO
El desarollo del proyecto fue util para mejor entender el proposito del system tables, especialmente en mi caso el de MS SQL SERVER. También fue utili para entender el formato de los DDL y como manejan los adminsitradores de RDBMS esta información para la base de datos.

---

## Desafios enfentrados y Soluciones
El desafío que encontre fu la complejidad de desarolla los DDL mayormente una forma de desarollarlos sin estar repitiendo codigo, al igual que entender las relaciones entre las tablas del sys. 

---

## Aprendizaje Clave
EL aprendizaje clave fue entender el manejo de la system tables para proyectar la metadata de los RDBMS. También desarollar mejor conocimiento sobre las estructuras de las Administradores de RDBMS.
