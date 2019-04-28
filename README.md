# MovieLister
MovieLister is a console program to help you quickly identify new DVD releases available to torrent.

To see a list of movies, use "movielister" from the command line.

Use "movielister ignore [Name]" to ignore a movie. It will never be displayed again.

![Screenshot](https://i.imgur.com/KwsQavf.png)

# Installation
1) Place built code in any folder.
2) Update your environment variables to point to the installed folder.
3) Done.

# How it works
1) Retrieves a list of the most-seeded torrents from piratebay (source can be changed).
2) Movies on your ignore list are filtered out.
3) TheMovieDB is used to fetch meta-information about remaining titles.
5) The top 5 highest rated moves are output to console.
