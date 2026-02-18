const visualTimerControl = {
    "intervalId":0,
    "secondsRemaining": 0,
    "start": function() {
        //Trigger the start of the chosen timer overlay
        startTimer();
        this.intervalId = setInterval(visualTimerControl.update, 1000);
    },
    "init": function() {
        this.processArgument();
        initTimer(this.secondsRemaining);
    },
    "update": function () {
        console.log(visualTimerControl.secondsRemaining);
        visualTimerControl.secondsRemaining = visualTimerControl.secondsRemaining - 1;
        if (visualTimerControl.secondsRemaining < 0) {
            visualTimerControl.finish();
        } else {
            updateTimer(visualTimerControl.secondsRemaining);
        }
    },
    "finish": function () {
        clearInterval(visualTimerControl.intervalId);
        finishTimer();
        console.log("Done");
    },
    "processArgument": function() {
        if (window.location.hash.length > 0) {
            this.secondsRemaining = window.location.hash.substring(1);
        } else {
            this.secondsRemaining = 10;
        }
    }
};





document.addEventListener('DOMContentLoaded', (event) => {
    visualTimerControl.init();
});

window.addEventListener('load', (event) => {
    visualTimerControl.start();
});