# Community and Support

MonoGame.Extended has a growing community of users from all around the globe. We've got lots of ways you can ask questions and get help about the library.

 - Of course, there's [this documentation](http://craftworkgames.github.io/MonoGame.Extended/). By contributing to it you're making it better for everyone.
 - We've got our own section on the [MonoGame Community Forums](http://community.monogame.net/c/extended)
 - We've even got a [tag on stackoverflow](http://gamedev.stackexchange.com/questions/tagged/monogame-extended) ;)
 - Development is usually discussed in [github issues](https://github.com/craftworkgames/MonoGame.Extended/issues)
 - We've got a live chatroom on [gitter](https://gitter.im/craftworkgames/MonoGame.Extended)

Note: When you ask a question, please consider how permanent the answer should be. If it's something that might benefit others choose the docs, forum or stack overflow over live chat.

# Docs

## We need your help writing better documentation
 - Good documentation is what separates good open source projects from great ones. 
 - Documentation is a community effort.
 - Even a little bit of documentation is better than none.
 - A rough draft is better than none.
 - If we all wrote a little documentation, it adds up to a lot.


## How to contribute to the documentation

The docs are located in the source [Docs/](https://github.com/craftworkgames/MonoGame.Extended/tree/develop/Docs) folder.  They are built with [MkDocs](http://www.mkdocs.org/#installation).

 - If you spot an error in the documentation, fix it. This is the simplest way to contribute.
 - If you notice some missing or out of date information, add it. This is the next simplest way to contribute.
 - If there's something completely missing, write a stub page. It's a start.
 - If you want to go a step further, write a tutorial or a whole new page.
 - If you have any trouble editing the docs, please [tell somebody](#community-and-support).
 - You can edit the page directly on Github, it's Markdown, so you don't necessesarily need to install MkDocs to contribute.

### Getting started with MKDocs

Running your own dev docs server at http://127.0.0.1:8000 (your computer) can be done by installing python, and then installing mkdocs.

```bash
> python -m pip install mkdocs
```

To run your local server run the mkdocs module with the `serve` command.

```bash
> cd MonoGame.Extended
MonoGame.Extended> python -m mkdocs serve
INFO    -  Building documentation...
INFO    -  Cleaning site directory
[I 170608 01:12:06 server:283] Serving on http://127.0.0.1:8000
[I 170608 01:12:06 handlers:60] Start watching changes
[I 170608 01:12:06 handlers:62] Start detecting changes
[I 170608 01:12:15 handlers:133] Browser Connected: http://127.0.0.1:8000/
```
Now every time you edit a `*.md` file in the `Docs/` folder the site will refresh with the latest info.

To add a new page, you must add Markdown file, and then reference it in the `mkdocs.yml` file somewhere appropriate. 

> All pages must be somewhere in the mkdocs.yml `pages` section, otherwise links to it will not work.
