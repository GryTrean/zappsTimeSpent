
![ZAWAPPS_LOGO](https://i.imgur.com/2NkZSmV.png)

# CS2 - zappsTimeSpent

A simple CS2 CounterStrikeSharp plugin to track player time spent in-game with possibility to check time spent in each team (T, CT or Spect).

### PAID REQUESTS
I'm taking paid CS2 plugin requests. Contact me via Discord (krystian01) or email: grytrean@gmail.com if interested.

### DEPENDENCIES
The following dependencies are required for the plugin to work correctly:
- CounterStrikeSharp

### AUTHOR
- [GryTrean](https://github.com/GryTrean) - developer

### TRANSLATIONS
```json
{
    "timespent.show.prefix": "{silver}[ {lime}TimeSpent{silver} ]",
    "timespent.show.failload": "{lightred}Failed loading data.",
    "timespent.show.failloadplayer": "{lightred}Failed to load data about this player.",
    "timespent.show.description": "{default}Time spent by {gold}{1} {default}in the last {gold}{1} {default}days",
    "timespent.show.description.alltime": "{default}Time spent by {gold}{0}"
}
```

### CONFIGS
```json
{
  "Database Settings": {
    "Hostname": "localhost",
    "Username": "root",
    "Password": "password",
    "Port": 3306,
    "Database Name": "database",
    "sslmode": "none",
    "table-prefix": ""
  },
  "Command Settings": {
    "Check Time Commands": [
      "css_checktime",
    ],
    "Check Time Command Description": "Check player time spent"
  },
  "Time interval": 60, -- How often to add player timespent to player
  "Force save Data Interval": 600, -- How often to force a database save of all online players
  "CheckTime Command Response": [
    "{PREFIX} {DESCRIPTION}",
    "{gold}\u00BB Total time: {TOTAL_TIME}",
    "{gold}\u00BB Terrorist time: {T_TIME}",
    "{gold}\u00BB Counter-Terrorist time: {CT_TIME}",
    "{gold}\u00BB IN-TEAM TIME (CT/T): {TEAM_TIME}",
    "{gold}\u00BB Spectator time: {SPEC_TIME}"
  ],
  "ConfigVersion": 1
}
```
The config file gets automatically generated on first plugin start.

The plugin saves player data to the database when a player disconnects from the server. It keeps track of the current session time and only saves it to the database when the player disconnects from the server to reduce the amount of SQL queries. However, in case of server crashes, a "Force save Data Interval" config is included which is the time, in seconds, between each forced save of all players online. This is added to reduce the chance lost data if the server were to crash or otherwise cause a player to disconnect without firing the Player Disconnect event through CSSharp.
