# CsvImporter for Acme Corporation

En este proyecto se lee un fichero .csv almacenado en una cuenta de almacenamiento de Azure e inserte su contenido en una BD SQL Server local.

# Arquitectura
El proyecto esta desarrollado usando [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) ya que es un modelo que hace mucho uso de la inyección de dependencias permitiendonos un alto desacoplamiento entre capas, esto nos permite tambie sacarle un mayor provecho a un framework como .net Core soporta la inyección de dependencias de forma nativa.
Elejí esta arquitectura debido a que permite un alto desacoplamiento permitiéndonos reutilizar muchas funcionalidades al momento de resolver problemas similares. Por ejemplo, nos permite tener diferentes implementaciones de una clase que devuelva un archivo, con la diferencia de que una clase tiene la lógica para leer un archivo en la nube y otra clase tiene la lógica para leer un archivo del disco duro. Al tener clases desacopladas podemos intercambiar fácilmente las clases sin hacer mayores cambios en las clases que hagan uso de ellas.

# Infraestructura
En cuanto a la Infraestructura me puse a investigar un poco como se resuelve la problemática de insertar grandes volúmenes de datos. Encontre [éste articulo](https://timdeschryver.dev/blog/faster-sql-bulk-inserts-with-csharp) de [@tim_deschryver](https://twitter.com/tim_deschryver) donde realiza una comparativa con diferentes metodeos y volumenes de datos llegando a la conclución de que el mas óptimo es utilizar SQL Bulk Copy.

![Resultado de pruebas de rendimiento](https://timdeschryver.dev/blog/faster-sql-bulk-inserts-with-csharp/images/chart.jpg)

# Consumo de Datos
El consumo de datos proveniente de un archivo puede realizarse de dos formas. La clase [GetFileFactory.cs](https://github.com/pablo-pereira/PruebaCsvImporter_PabloPereira/blob/main/CsvImporter.Infraestructure/Factory/GetFileFactory.cs) implementa el pattron Factory, dependiendo del formato de la ruta del archivo que le llegue devuellve una instancia u otra. Si la ruta es una URL devuelve una instancia de la clase [GetFileFromUrl.cs](https://github.com/pablo-pereira/PruebaCsvImporter_PabloPereira/blob/main/CsvImporter.Infraestructure/Services/GetFileFromUrl.cs), que tiene la logica para leer el archivo en la nube. Si la ruta no es una URL entonces devuelve una instancia de la clase [GetFileFactory.cs](https://github.com/pablo-pereira/PruebaCsvImporter_PabloPereira/blob/main/CsvImporter.Infraestructure/Services/GetFile.cs) que simplemente lee el archivo del disco. Ambas clases tienen la misma interfaz por lo tanto son intercambiables sin problemas.

# Inserción de Datos
La inserción de datos se hace utilizando `Microsoft.Data.SqlClient` y `SqlBulkCopy`. La inserción se realiza en lotes de 100000 registros cuando se lee el archivo localmente y de 5000 cuando se lee de la nube. Realizando pruebas de rendimiento con una muestra en un archivo local de 2 millones de registros noté que el tamaño del lote no afecta mucho al tiempo de ejecución pero sí al uso de memoria y procesador, por lo tanto jugando con ese valor se puede ir adaptando el uso de recursos. 
Cabe aclarar que la ventana de registros en el caso de esta leyendo el archivo en la nube es de 5000, a diferencia de los 100000 cuando el archivo es local, la elegí luego de hacer pruebas y notar que al ser la velocidad de conexión un cuello de botella, si la ventana es grande el consumo de recursos fluctua al ir acumulandose en memoria los registros hasta ser insertados en la base de datos. con una ventana de 5000 registros se mantiene estable el uso de memoria y procesador.

# Soluciones Descartadas
Creo que puede haber soluciones más óptimas para escenarios como este donde se tiene que consumir grandes cantidades de datos desde la nube. La solución que pensé pero no pude implementar por no tener las herramientas ni el tiempo necesario fue armar un store procedure en la base de datos que consuma directamente los datos de la nube.
```
  CREATE EXTERNAL DATA SOURCE data_source_name 
  WITH ( 
  TYPE = BLOB_STORAGE, 
  LOCATION = 'https://storage_account_name.blob.core.windows.net/container_name Jump '
  [, CREDENTIAL = credential_name ]
  )
```
 
```
  BULK INSERT Sales.Invoices
  FROM 'inv-2017-12-08.csv'
  WITH (DATA_SOURCE = 'MyAzureStorage');
 ```
 
 [(Fuente)](https://social.technet.microsoft.com/wiki/contents/articles/52061.t-sql-bulk-insert-azure-csv-blob-into-azure-sql-database.aspx)
 
 # Librerías Usadas
 - Serilog para los logs.
 - Entity Framework para crear la tabla en la base de datos solamente.
 - MSTest y Moq para testing.

