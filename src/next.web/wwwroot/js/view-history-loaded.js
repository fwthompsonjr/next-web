function initializeHistoryBox() {
    try {
        if (!historybox || !historybox.fetch || !historybox.init_filters) { return; }
        historybox.fetch.item(0);
        // historybox.init_filters();
    }
    catch {
    }
}


initializeHistoryBox();