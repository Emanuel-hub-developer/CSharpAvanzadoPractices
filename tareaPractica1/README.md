En esta tarea se encuentra una TareaAPI creada mediante unos requerimientos,acontinuacion estaré detallando su funcionamiento.

DESCRIPCION DE LOS METODOS HTTP:

Metodos con los que cuenta la API llamada "TareaAPI"

![image alt](https://github.com/Emanuel-hub-developer/CSharpAvanzadoPractices/blob/87a5d69fb7ba2aed1d56205284b2b1d720c1de01/tareaPractica1/ImagesReferencesForDocumentation/swaggerTareaApi.png)

TareaAPI cuenta con metodos http que son los siguientes:

GET: /api/obtenerTareas , obtiene todas las tareas que hay en la base de datos, como se puede ver en la siguiente demostracion:

![image alt](https://github.com/Emanuel-hub-developer/CSharpAvanzadoPractices/blob/87a5d69fb7ba2aed1d56205284b2b1d720c1de01/tareaPractica1/ImagesReferencesForDocumentation/getApiTarea.png)

POST: /api/crearTarea , permite crear tareas en la base de datos, como se puede ver en la siguiente demostracion:

![image alt](https://github.com/Emanuel-hub-developer/CSharpAvanzadoPractices/blob/87a5d69fb7ba2aed1d56205284b2b1d720c1de01/tareaPractica1/ImagesReferencesForDocumentation/postApiTarea.png)

PUT: /api/actualizarTarea , permite actualizar la informacion de las tareas mediante id, como se puede ver en la siguiente demostracion:

![image alt](https://github.com/Emanuel-hub-developer/CSharpAvanzadoPractices/blob/87a5d69fb7ba2aed1d56205284b2b1d720c1de01/tareaPractica1/ImagesReferencesForDocumentation/putApiTarea.png)

Vista en la Base de Datos:

ANTES DE ACTUALIZAR: 

![image alt](https://github.com/Emanuel-hub-developer/CSharpAvanzadoPractices/blob/87a5d69fb7ba2aed1d56205284b2b1d720c1de01/tareaPractica1/ImagesReferencesForDocumentation/dbQueryExample.png)

DESPÚES DE ACTUALIZAR: 

![image alt](https://github.com/Emanuel-hub-developer/CSharpAvanzadoPractices/blob/87a5d69fb7ba2aed1d56205284b2b1d720c1de01/tareaPractica1/ImagesReferencesForDocumentation/dbExampleAfterPut.png)

DELETE: /api/eliminarTarea , permite eliminar tareas mediante el id, como se puede ver en la siguiente demostracion:

![image alt](https://github.com/Emanuel-hub-developer/CSharpAvanzadoPractices/blob/87a5d69fb7ba2aed1d56205284b2b1d720c1de01/tareaPractica1/ImagesReferencesForDocumentation/deleteApiTarea.png)

Vista en la Base de Datos: 

![image alt](https://github.com/Emanuel-hub-developer/CSharpAvanzadoPractices/blob/87a5d69fb7ba2aed1d56205284b2b1d720c1de01/tareaPractica1/ImagesReferencesForDocumentation/dbExampleAfterDelete.png)


-----------------------------------------------------------FIN DE LA DOCUMENTACION----------------------------------------------------






