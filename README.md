# MovieLister
MovieLister is a console program to help you quickly identify new DVD releases. 

It searches ThePirateBay and IMDB for new releases not on your ignore list.

| Command        | Description   |
| ------------- |-------------|
| \>movielist     | Get list of movies |
| \>movielist ignore [name] | Hide the specified movie from all future lists |
| \>movielist ignored | Show list of ignored movies |

![Screenshot](https://i.imgur.com/KwsQavf.png)

# Installation
1) Run the installer (Release\Setup.msi)
2) Update your environment path variable to point to the installed folder.
3) Settings can be tweaked in the config.json file. See comments in file for details.

# How it works
1) Retrieves a list of the most-seeded torrents from piratebay (source can be changed in config.json).
2) Extends the list of movies with any new DVD releases found on IMDB.
3) Movies on your ignore list are filtered out.
4) TheMovieDB is used to fetch meta-information about remaining titles.
5) The top 5 highest rated moves are output to console.

# Ignored Movies
* Partial matches work for ignored movies. 
* Year is also not requred, but will help make the filter more specific.
* Example: "Spider-Man (2002)", "Spider-Man", and "Spider (2002)" all filter Spider-Man (2002).

Ignored movies are saved to AppData: '%AppData%\MovieLister\IgnoredMovies.txt'
