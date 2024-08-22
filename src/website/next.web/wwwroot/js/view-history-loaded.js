function initializeHistoryBox() {
    if (!historybox || !historybox.fetch || !historybox.init_filters) { return; }
    historybox.fetch.item(0);
}


initializeHistoryBox();