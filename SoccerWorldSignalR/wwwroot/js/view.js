//client-side callbacks
connection.on("OnWorldStateChanged", function (obj) {
    obj = JSON.parse(obj);
    SetWorldState(obj);
});
connection.on("OnCompetitionEventProcessed", function (obj) {
    obj = JSON.parse(obj);
    if (obj.CompetitionId == $('#competition').val()) {
        //user is watching current competition, and competition got update
        //so update data
        RequestCompetitionData();
    }
    //TODO: add competitionevent to newsticker
});
connection.on("OnMatchesChanged", function (obj) {
    obj = JSON.parse(obj);
    UpdateMatches(obj);
});
connection.on("OnMatchesEnd", function (obj) {
    obj = JSON.parse(obj);
    UpdateMatches(obj);
});
connection.on("OnCompetitionStandingsChanged", function (obj) {
    obj = JSON.parse(obj);
    if (obj.CompetitionId == $('competition').val())
        SetLeagueTable(obj.LeagueTable);
});
connection.on("OnCompetitionsDataRequest", function (obj) {
    obj = JSON.parse(obj);
    SetCompetitions(obj);
    RequestCompetitionData();
});
connection.on("OnCompetitionDataRequest", function (obj) {
    obj = JSON.parse(obj);
    SetSeasons(obj.CompetitionSeasons);
    SetLeagueTable(obj.LeagueTable);
    SetMatches(obj.Matches);
});


//server invocations
function ContinueWorld() {
    connection.invoke("ContinueWorld");
}
function RequestCompetitionsData() {
    var country_id = $('#country').val();
    connection.invoke("CompetitionsDataRequest", parseInt(country_id));
}
function RequestCompetitionData() {
    var comp_id = $('#competition').val();
    var season = $('#season').val();
    connection.invoke("CompetitionDataRequest", parseInt(comp_id), parseInt(season), null);
}

//client-side updatefunctions
function SetWorldState(state) {
    console.log(state);
    $('#world_date').text(state.CurrentDateTime);
    if (state.AsyncProcessesCount > 0) {
        $('#world_processing').removeClass('invisible');
        $('#world_continue_button').attr('disabled', true);
    }
    else {
        $('#world_processing').addClass('invisible');
        $('#world_continue_button').attr('disabled', false);
    }
}

function SetCompetitions(competitions) {
    var competition_selector = $('#competition');
    competition_selector.empty();
    for (var i in competitions) {
        competition_selector.append(
            $('<option value="' + competitions[i].Id + '">' + competitions[i].Name + '</option>')
        );
    }
}

function SetSeasons(seasons) {
    var seasons_selector = $('#season');
    seasons_selector.empty();
    for (var i in seasons) {
        seasons_selector.append(
            $('<option value="' + seasons[i] + '">' + seasons[i] + '</option>')
        );
    }
}

function SetLeagueTable(table) {
    var view = $('#competitiontable_view');
    
    //build new table-elements
    var arraylist = [];

    for (i in table) {
        var newrow = $('<div class="w-100 standings_element"></div>');
        newrow.append(
            '<span class="col-3 col-sm-5 standings_club">' + table[i].ClubName+'</span>',
            '<span class="col-1 standings_W" style="">' + table[i].Won+'</span>',
            '<span class="col-1 standings_T" style="">' + table[i].Tied+'</span>',
            '<span class="col-1 standings_L" style="">' + table[i].Lost+'</span>',
            '<span class="col-1 standings_GF" style="">' + table[i].GoalsFor+'</span>',
            '<span class="col-1 standings_GA" style="">' + table[i].GoalsAgainst+'</span>',
            '<span class="col-1 standings_points" style="">' + table[i].Points+'</span>'
        );
        arraylist.push(newrow);
    };
    
    //replace all current elements shown
    view.empty();
    view.append(arraylist);
}
function SetMatches(matches) {
    var view = $('#matches_view');

    //build new table-elements
    var arraylist = [];

    if (matches.length > 0)
        arraylist.push(
            $('<div class="row"><span class="col-3 col-sm-4">' + matches[0].Date + '</span></div>')
        );

    for (var i in matches) {
        var newrow = $('<div id="match_' + matches[i].Id + '" class="row"></div>');
        
        newrow.append(
            '<span class="col-2 col-sm-4">' + matches[i].HomeClub + '</span>',
            '<span class="col-1 col-sm-2">' + matches[i].HomeScore + ' - ' + matches[i].AwayScore + '</span>',
            '<span class="col-2 col-sm-4">' + matches[i].AwayClub + '</span>',
            '<span class="col-1 col-sm-1">' + matches[i].MinutesPlayed + '</span>'
        );
        arraylist.push(newrow);
    };

    //replace all current elements shown
    view.empty();
    view.append(arraylist);
}

function UpdateMatches(matches) {
    for (var i in matches) {
        UpdateMatch(matches[i]);
    }
}
function UpdateMatch(match) {
    var match_view = $('#match_' + match.Id);
    if (match_view.length == 1) {
        if (match.IsFinished)
            match_view.addClass('finished_match');
        match_view.empty();
        match_view.append(
            '<span class="col-2 col-sm-4">' + match.HomeClub + '</span>',
            '<span class="col-1 col-sm-2">' + match.HomeScore + ' - ' + match.AwayScore + '</span>',
            '<span class="col-2 col-sm-4">' + match.AwayClub + '</span>',
            '<span class="col-1 col-sm-1">' + match.MinutesPlayed + '</span>'
        );
        
    }
}