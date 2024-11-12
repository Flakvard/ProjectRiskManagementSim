// Function to switch between the simulation button and the dashboard link
function initializeSim() {
    const runSimButton = document.getElementById('run-sim-button');
    const runSimLink = document.getElementById('run-sim-link');

    if (runSimButton) {
        runSimButton.addEventListener('click', () => {
            runSimLink.classList.remove('hidden')
            runSimButton.classList.add('hidden')
        })
    }
    if (runSimLink) {
        runSimLink.addEventListener('click', () => {
            runSimButton.classList.remove('hidden')
            runSimLink.classList.add('hidden')
        })
    }
}
initializeSim();

// Attach the function to Htmx events
document.addEventListener('htmx:beforeRequest', function (event) {
    if (event.detail.elt.id === 'run-sim-button' || event.detail.elt.id === 'run-sim-link') {
        initializeSim();
    }
});
document.addEventListener('initializeSim', function (event) {
    if (event.detail.elt.id === 'run-sim-button' || event.detail.elt.id === 'run-sim-link') {
        initializeSim();
    }
});


function initializeModal() {
    const openModal = document.getElementById('openModal');
    const openUpdateModal = document.getElementById('openUpdateModal');
    const modal = document.getElementById('modal');
    const closeModal = document.getElementById('closeModal');
    const closeModalFooter = document.getElementById('closeModalFooter');

    const modalWarn = document.getElementById('modal-warn');
    const openWarningModal = document.getElementById('openWarningModal');
    const closeModalWarn = document.getElementById('closeModalWarn');
    const closeModalWarnFooter = document.getElementById('closeModalWarnFooter');


    if (openModal) {
        openModal.addEventListener('click', () => {
            modal.classList.remove('hidden');
            document.body.classList.add('overflow-hidden'); // Prevent scrolling on body
        });
    }

    if (openUpdateModal) {
        openUpdateModal.addEventListener('click', () => {
            modal.classList.remove('hidden');
            document.body.classList.add('overflow-hidden'); // Prevent scrolling on body
        });
    }
    if (openWarningModal) {
        openWarningModal.addEventListener('click', () => {
            modalWarn.classList.remove('hidden');
            document.body.classList.add('overflow-hidden'); // Prevent scrolling on body
        });
    }

    if (closeModalWarnFooter) {
        closeModalWarnFooter.addEventListener('click', () => {
            modalWarn.classList.add('hidden');
            document.body.classList.remove('overflow-hidden'); // Re-enable scrolling
        });
    }
    if (closeModalWarn) {
        closeModalWarn.addEventListener('click', () => {
            modalWarn.classList.add('hidden');
            document.body.classList.remove('overflow-hidden'); // Re-enable scrolling
        });
    }


    if (closeModal) {
        closeModal.addEventListener('click', () => {
            modal.classList.add('hidden');
            document.body.classList.remove('overflow-hidden'); // Re-enable scrolling
        });
    }

    if (closeModalFooter) {
        closeModalFooter.addEventListener('click', () => {
            modal.classList.add('hidden');
            document.body.classList.remove('overflow-hidden'); // Re-enable scrolling
        });
    }

    // Close modal if clicking outside of the modal content
    window.addEventListener('click', (e) => {
        if (e.target === modal) {
            modal.classList.add('hidden');
            document.body.classList.remove('overflow-hidden'); // Re-enable scrolling
        }
    }); 
    // Close modal if clicking outside of the modal content
    window.addEventListener('click', (e) => {
        if (e.target === modalWarn) {
            modalWarn.classList.add('hidden');
            document.body.classList.remove('overflow-hidden'); // Re-enable scrolling
        }
    });
}

// Attach the event listener for HTMX afterSwap event
document.addEventListener('htmx:afterSwap', (event) => {
    if (event.detail.target.id === 'modal' || event.detail.target.id === 'modal-warn') {
        initializeModal();
    }
});
// Attach the event listener for HTMX afterSwap event
document.addEventListener('htmx:afterSettle', (event) => {
    if (event.detail.target.id === 'modal' || event.detail.target.id === 'modal-warn') {
        initializeModal();
    }
});

document.body.addEventListener("initializeModal", function (evt) {
    initializeModal();
})


// Initial call to initialize the modal when the page loads
initializeModal();

let columnCount = 11; // Initial count based on the provided columns

function addColumn() {
    columnCount++;
    const form = document.getElementById('kanbanForm');
    const newColumn = document.createElement('div');
    newColumn.innerHTML = `
                <div class="column-input-border">
                  <div class="flex flex-row gap-2 justify-center items-center>
                    <label class="label-input" for="name${columnCount}">Name:</label>
                    <input class="input-border" type="text" id="name${columnCount}" name="name${columnCount}">
                  </div>
                  <div class="flex flex-row gap-2 justify-center items-center>
                    <label class="label-input" for="wip${columnCount}">WIP:</label>
                    <input class="input-border" type="number" id="wip${columnCount}" name="wip${columnCount}">
                  </div>
                  <div class="flex flex-row gap-2 justify-center items-center>
                    <label class="label-input" for="wipMax${columnCount}">WIP Max:</label>
                    <input class="input-border" type="number" id="wipMax${columnCount}" name="wipMax${columnCount}">
                  </div>
                  <div class="flex flex-row gap-2 justify-center items-center>
                    <label class="label-input" for="lowBound${columnCount}">Estimated Low Bound:</label>
                    <input class="input-border" type="number" id="lowBound${columnCount}" name="lowBound${columnCount}">
                  </div>
                  <div class="flex flex-row gap-2 justify-center items-center>
                    <label class="label-input" for="highBound${columnCount}">Estimated High Bound:</label>
                    <input class="input-border" type="number" id="highBound${columnCount}" name="highBound${columnCount}">
                  </div>
                  <div class="flex flex-row gap-2 justify-center items-center>
                    <label class="label-input" for="isBuffer${columnCount}">Is Buffer:</label>
                    <input class="input-border" type="checkbox" id="isBuffer${columnCount}" name="isBuffer${columnCount}">
                  </div>
                </div>
            `;
    form.insertBefore(newColumn, document.getElementById('addColumnButton'));
}
