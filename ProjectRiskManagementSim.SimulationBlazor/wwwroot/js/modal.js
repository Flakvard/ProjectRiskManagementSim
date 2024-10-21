<!-- Script to handle modal open/close functionality -->
function initializeModal() {
  const openModal = document.getElementById('openModal');
  const modal = document.getElementById('modal');
  const closeModal = document.getElementById('closeModal');
  const closeModalFooter = document.getElementById('closeModalFooter');

  if (openModal) {
    openModal.addEventListener('click', () => {
      modal.classList.remove('hidden');
      document.body.classList.add('overflow-hidden'); // Prevent scrolling on body
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

  <!-- To handle Columns in form -->
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
}

// Attach the event listener for HTMX afterSwap event
document.addEventListener('htmx:afterSwap', (event) => {
  if (event.detail.target.id === 'modal') {
    initializeModal();
  }
});

// Initial call to initialize the modal when the page loads
initializeModal();
