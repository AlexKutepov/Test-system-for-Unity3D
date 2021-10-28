# Unity3d Test System 

The target language is English.

Version Unity3d is all versions that support the TextMeshPro,
but you can rewrite the code and remove TextMeshPro.

The design pattern used is MVC. The answers are generated automatically.

How to use it? On scene the "Testing" exist the GameObject "TestSystem" which used to add new questions.
TestSystem includes the GameObject "Controller" which controls the UI components.
You can add new questions in the Scene or use the XML parser, script Test System includes it the opportunity. For example, you can use in the first step the method "saveQuestionsToFile" through console Unity3D and get the XML file. The next step is adding new questions into file XML. And after adding you can use the method  "loadQuestionsFromFile"

What the tests look like:

[![Start menu](https://sun9-37.userapi.com/impg/yAgZlIeyjKx4KvQIqYR8yUvH0XOyzuuB2UaJ0w/q466t38JWAw.jpg?size=926x618&quality=95&sign=dcf273b1945955fd1e9bdfcd83b332a1&type=album "Start menu")](https://sun9-37.userapi.com/impg/yAgZlIeyjKx4KvQIqYR8yUvH0XOyzuuB2UaJ0w/q466t38JWAw.jpg?size=926x618&quality=95&sign=dcf273b1945955fd1e9bdfcd83b332a1&type=album "Start menu")

a question can have many answers, they look like this:

[![](https://sun9-82.userapi.com/impg/R-51gWgX3O1NsrO4OcZIG2vbGG8ujS6jxUpGfQ/PJNZCyOn6Gk.jpg?size=926x618&quality=95&sign=7a8087eedc692d6b914dae1a169e65dd&type=album)](https://sun9-82.userapi.com/impg/R-51gWgX3O1NsrO4OcZIG2vbGG8ujS6jxUpGfQ/PJNZCyOn6Gk.jpg?size=926x618&quality=95&sign=7a8087eedc692d6b914dae1a169e65dd&type=album)

In the finish you will see this menu:

[![](https://sun9-72.userapi.com/impg/1ij01MfrimZs3JDbvJQChsIDoigfrRUVstL_jQ/UY02gWKkmhE.jpg?size=948x640&quality=95&sign=75645d666b41fafdb6b7c8a5f39a1d3a&type=album)](https://sun9-72.userapi.com/impg/1ij01MfrimZs3JDbvJQChsIDoigfrRUVstL_jQ/UY02gWKkmhE.jpg?size=948x640&quality=95&sign=75645d666b41fafdb6b7c8a5f39a1d3a&type=album)

To set up the time you can change the CultureInfo in the TestController.
the button "Save rezult" don't have a method.


## License

MIT

**Free Software, Hell Yeah!**
