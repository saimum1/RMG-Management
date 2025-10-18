
        let declarationSettingsId = null;
        let originalQuestions = [];
        let autoSaveTimeout = null;
        let isSaving = false;

        // Load declaration settings when page loads
        document.addEventListener('DOMContentLoaded', function() {
            loadDeclarationSettings();
        });

        function loadDeclarationSettings() {
            const questionsList = document.getElementById('questions-list');
            questionsList.innerHTML = '<div class="loading"><div class="spinner"></div></div>';
            
            fetch('/api/declaration-settings')
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Failed to load declaration settings');
                    }
                    return response.json();
                })
                .then(data => {
                    // Find settings for Next Source Ltd
                    const companySettings = data.find(setting => setting.companyName === 'Next Source Ltd');
                    
                    if (companySettings) {
                        declarationSettingsId = companySettings.id;
                        originalQuestions = [...companySettings.questions];
                        renderQuestions(companySettings.questions);
                        
                        // Change submit button text to "Update"
                        document.getElementById('submit-btn').textContent = 'Update';
                    } else {
                        // No settings found for this company
                        declarationSettingsId = null;
                        originalQuestions = [];
                        renderQuestions([]);
                        
                        // Reset submit button text to "Submit"
                        document.getElementById('submit-btn').textContent = 'Submit';
                    }
                })
                .catch(error => {
                    console.error('Error loading declaration settings:', error);
                    showToast('Error loading declaration settings');
                    renderQuestions([]);
                    
                    // Reset submit button text to "Submit" in case of error
                    document.getElementById('submit-btn').textContent = 'Submit';
                });
        }

        function renderQuestions(questions) {
            const questionsList = document.getElementById('questions-list');
            questionsList.innerHTML = '';
            
            if (questions.length === 0) {
                questionsList.innerHTML = '<p style="text-align: center; color: var(--text-secondary);">No questions added yet</p>';
                addNewQuestionInput();
                return;
            }
            
            questions.forEach((question, index) => {
                const questionBox = document.createElement('div');
                questionBox.className = 'question-box';
                questionBox.innerHTML = `
                    <input type="text" class="question-input" value="${question}" onchange="questionChanged(${index})" onkeyup="handleKeyUp(event, ${index})">
                    <div class="question-actions">
                        <button class="action-btn delete-btn" onclick="deleteQuestion(${index})" title="Delete">
                            🗑️
                        </button>
                    </div>
                `;
                questionsList.appendChild(questionBox);
            });
            
            addNewQuestionInput();
        }

        function addNewQuestionInput() {
            const questionsList = document.getElementById('questions-list');
            const newQuestionBox = document.createElement('div');
            newQuestionBox.className = 'question-box';
            newQuestionBox.innerHTML = `
                <input type="text" class="question-input" placeholder="Enter question text" onchange="questionChanged(-1)" onkeyup="handleKeyUp(event, -1)">
                <div class="question-actions">
                    <button class="action-btn delete-btn" onclick="removeQuestion(this)" title="Remove">
                        🗑️
                    </button>
                </div>
            `;
            questionsList.appendChild(newQuestionBox);
        }

        function addQuestion() {
            const questionsList = document.getElementById('questions-list');
            const lastQuestionBox = questionsList.lastElementChild;
            const input = lastQuestionBox.querySelector('.question-input');
            
            if (input && input.value.trim() !== '') {
                // Create a new question box with the input value
                const newQuestionBox = document.createElement('div');
                newQuestionBox.className = 'question-box';
                const newIndex = questionsList.children.length - 1; // -1 because we're adding before the empty input
                newQuestionBox.innerHTML = `
                    <input type="text" class="question-input" value="${input.value.trim()}" onchange="questionChanged(${newIndex})" onkeyup="handleKeyUp(event, ${newIndex})">
                    <div class="question-actions">
                        <button class="action-btn delete-btn" onclick="deleteQuestion(${newIndex})" title="Delete">
                            🗑️
                        </button>
                    </div>
                `;
                
                // Insert before the last element (the input box)
                questionsList.insertBefore(newQuestionBox, lastQuestionBox);
                
                // Clear the input
                input.value = '';
                input.focus();
                
                // Trigger auto-save
                questionChanged(-1);
            } else {
                // Just add a new empty input box
                addNewQuestionInput();
                
                // Focus on the new input
                const newInput = questionsList.lastElementChild.querySelector('.question-input');
                newInput.focus();
            }
        }

        function handleKeyUp(event, index) {
            // If Enter key is pressed
            if (event.key === 'Enter') {
                event.preventDefault();
                addQuestion();
            }
        }

        function questionChanged(index) {
            // Clear any existing timeout
            if (autoSaveTimeout) {
                clearTimeout(autoSaveTimeout);
            }
            
            // Set a new timeout to auto-save after 1 second of inactivity
            autoSaveTimeout = setTimeout(() => {
                autoSave();
            }, 1000);
        }

        function autoSave() {
            if (isSaving) return;
            
            // Collect all questions
            const questions = [];
            const questionBoxes = document.querySelectorAll('.question-box');
            
            questionBoxes.forEach(box => {
                const questionInput = box.querySelector('.question-input');
                
                if (questionInput && questionInput.value.trim() !== '') {
                    questions.push(questionInput.value.trim());
                }
            });
            
            if (questions.length === 0) {
                return;
            }
            
            // Prepare data for API
            const declarationData = {
                companyName: 'Next Source Ltd',
                questions: questions
            };
            
            // Determine if we're creating or updating
            const url = declarationSettingsId 
                ? `/api/declaration-settings/${declarationSettingsId}` 
                : '/api/declaration-settings';
            const method = declarationSettingsId ? 'PUT' : 'POST';
            
            isSaving = true;
            
            // Send data to API
            fetch(url, {
                method: method,
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(declarationData),
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log('Auto-save successful:', data);
                
                // Update the ID if we just created a new setting
                if (!declarationSettingsId) {
                    declarationSettingsId = data.id;
                    // Change submit button text to "Update"
                    document.getElementById('submit-btn').textContent = 'Update';
                }
                
                // Update original questions
                originalQuestions = [...questions];
                
                // Show save indicator
                const saveIndicator = document.getElementById('save-indicator');
                saveIndicator.style.display = 'inline-block';
                setTimeout(() => {
                    saveIndicator.style.display = 'none';
                }, 2000);
            })
            .catch(error => {
                console.error('Error auto-saving declaration settings:', error);
                showToast('Error saving declaration settings');
            })
            .finally(() => {
                isSaving = false;
            });
        }

        function deleteQuestion(index) {
            const questionBoxes = document.querySelectorAll('.question-box');
            const questionBox = questionBoxes[index];
            
            questionBox.remove();
            showToast('Question deleted');
            
            // Renumber the remaining questions
            updateQuestionIndices();
            
            // Trigger auto-save
            questionChanged(-1);
        }

        function removeQuestion(button) {
            const questionBox = button.closest('.question-box');
            questionBox.remove();
            
            // Renumber the remaining questions
            updateQuestionIndices();
            
            // Trigger auto-save
            questionChanged(-1);
        }

        function updateQuestionIndices() {
            const questionBoxes = document.querySelectorAll('.question-box');
            
            questionBoxes.forEach((box, index) => {
                const deleteBtn = box.querySelector('.delete-btn');
                
                if (deleteBtn) {
                    deleteBtn.setAttribute('onclick', `deleteQuestion(${index})`);
                }
                
                const input = box.querySelector('.question-input');
                if (input) {
                    input.setAttribute('onchange', `questionChanged(${index})`);
                    input.setAttribute('onkeyup', `handleKeyUp(event, ${index})`);
                }
            });
        }

        function submitForm() {
            // Collect all questions
            const questions = [];
            const questionBoxes = document.querySelectorAll('.question-box');
            
            questionBoxes.forEach(box => {
                const questionInput = box.querySelector('.question-input');
                
                if (questionInput && questionInput.value.trim() !== '') {
                    questions.push(questionInput.value.trim());
                }
            });
            
            if (questions.length === 0) {
                showToast('Please add at least one question');
                return;
            }
            
            // Prepare data for API
            const declarationData = {
                companyName: 'Next Source Ltd',
                questions: questions
            };
            
            // Determine if we're creating or updating
            const url = declarationSettingsId 
                ? `/api/declaration-settings/${declarationSettingsId}` 
                : '/api/declaration-settings';
            const method = declarationSettingsId ? 'PUT' : 'POST';
            
            // Send data to API
            fetch(url, {
                method: method,
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(declarationData),
            })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log('Success:', data);
                
                // Update the ID if we just created a new setting
                if (!declarationSettingsId) {
                    declarationSettingsId = data.id;
                    // Change submit button text to "Update"
                    document.getElementById('submit-btn').textContent = 'Update';
                }
                
                // Update original questions
                originalQuestions = [...questions];
                
                showToast('Declaration settings saved successfully!');
                
                // Re-render questions to ensure proper state
                renderQuestions(questions);
            })
            .catch(error => {
                console.error('Error:', error);
                showToast('Error saving declaration settings');
            });
        }
        
        function goBack() {
            showToast('Navigating back...');
            console.log('Going back to previous page');
            
            // Simulate navigation after a short delay
            setTimeout(() => {
                // window.history.back();
                console.log('Navigation would happen here');
            }, 500);
        }
        
        function showToast(message) {
            const toast = document.getElementById('toast');
            toast.textContent = message;
            toast.classList.add('show');
            
            setTimeout(() => {
                toast.classList.remove('show');
            }, 3000);
        }
