var plugin = {
    OpenTab : function(url)
    {
        url = UTF8ToString(url);
        window.open(url,'_blank');
    },
};
mergeInto(LibraryManager.library, plugin);