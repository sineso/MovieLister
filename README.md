# MovieLister
MovieLister is a console program to help you quickly identify new DVD releases. 

It searches ThePirateBay for the most seeded HD movies that are not on your ignore list.

To see a list of movies, use "movielist" from the command line.

Use "movielist ignore [Name]" to ignore a movie. It will never be displayed again.

![Screenshot](https://i.imgur.com/KwsQavf.png)

# Installation
1) Run the installer (Release\Setup.msi)
2) Update your environment path variable to point to the installed folder.

Ignored movies are saved to AppData:
'%AppData%\MovieLister\IgnoredMovies.txt'

# How it works
1) Retrieves a list of the most-seeded torrents from piratebay (source can be changed in config.json).
2) Movies on your ignore list are filtered out.
3) TheMovieDB is used to fetch meta-information about remaining titles.
4) The top 5 highest rated moves are output to console.
