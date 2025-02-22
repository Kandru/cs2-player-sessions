document.addEventListener('DOMContentLoaded', () => {
    const typeSelect = document.getElementById('type');
    const rulesContainer = document.getElementById('rules-container');
    const challengesList = document.getElementById('challenges-list');
    const uploadChallenges = document.getElementById('upload-challenges');
    let challenges = {};
    let allRules = {};
    let operators = [];

    fetch('data/challenge-types.json')
        .then(response => response.json())
        .then(data => {
            const { types, operators: ops } = data;
            operators = ops;
            allRules = types;
            for (const entry in allRules) {
                const option = document.createElement('option');
                option.value = entry;
                option.textContent = allRules[entry].name;
                typeSelect.appendChild(option);
            }

            typeSelect.addEventListener('change', () => {
                rulesContainer.innerHTML = '';
                const selectedType = typeSelect.value;
            });

            // Trigger change event to load rules for the default selected type
            typeSelect.dispatchEvent(new Event('change'));
        });

    document.getElementById('add-rule').addEventListener('click', () => {
        addRuleElement('', '', '');
    });

    document.getElementById('save-challenge').addEventListener('click', () => {
        const type = typeSelect.value;
        const title = document.getElementById('title').value;
        let slug = document.getElementById('slug').value;
        slug = slug.replace(/\s+/g, '_').replace(/[^a-zA-Z0-9_]/g, '');
        const points = document.getElementById('points').value;
        const amount = document.getElementById('amount').value;
        const rules = Array.from(document.querySelectorAll('#rules-container .rule')).map(ruleDiv => {
            const key = ruleDiv.querySelector('[name="key"]').value;
            const operator = ruleDiv.querySelector('[name="operator"]').value;
            if (ruleDiv.querySelector('[name="value"]').type == "checkbox") {
                var value = ruleDiv.querySelector('[name="value"]').checked;
            } else {
                var value = ruleDiv.querySelector('[name="value"]').value;
            }
            if (key && operator && value) {
                return { key, operator, value };
            }
            return null;
        }).filter(rule => rule !== null);

        const challenge = { title, type, points, amount, rules };
        challenges[slug] = challenge;

        const li = document.createElement('li');
        li.textContent = slug + " -> " + title;
        li.addEventListener('click', () => {
            typeSelect.value = challenge.type;
            document.getElementById('title').value = challenge.title;
            document.getElementById('slug').value = slug;
            document.getElementById('points').value = challenge.points;
            document.getElementById('amount').value = challenge.amount;
            rulesContainer.innerHTML = '';
            challenge.rules.forEach(rule => {
                addRuleElement(rule.key, rule.operator, rule.value);
            });
        });
        challengesList.appendChild(li);
    });

    document.getElementById('download-challenges').addEventListener('click', () => {
        const blob = new Blob([JSON.stringify({ blueprints: challenges }, null, 2)], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'challenges.json';
        a.click();
        URL.revokeObjectURL(url);
    });

    uploadChallenges.addEventListener('change', (event) => {
        const file = event.target.files[0];
        const reader = new FileReader();
        reader.onload = (e) => {
            const uploadedChallenges = JSON.parse(e.target.result).blueprints;
            challenges = { ...challenges, ...uploadedChallenges };
            challengesList.innerHTML = '';
            for (const slug in challenges) {
                const challenge = challenges[slug];
                const li = document.createElement('li');
                li.textContent = slug + " -> " + challenge.title;
                li.addEventListener('click', () => {
                    typeSelect.value = challenge.type;
                    document.getElementById('title').value = challenge.title;
                    document.getElementById('slug').value = slug;
                    document.getElementById('points').value = challenge.points;
                    document.getElementById('amount').value = challenge.amount;
                    rulesContainer.innerHTML = '';
                    challenge.rules.forEach(rule => {
                        addRuleElement(rule.key, rule.operator, rule.value);
                    });
                });
                challengesList.appendChild(li);
            }
        };
        reader.readAsText(file);
    });

    function addRuleElement(key = '', operator = '', value = '') {
        const ruleDiv = document.createElement('div');
        const keySelect = document.createElement('select');
        const operatorSelect = document.createElement('select');
        const valueInput = document.createElement('input');
        const deleteButton = document.createElement('button');
        ruleDiv.classList.add('rule');
        // Add key select
        keySelect.name = 'key';
        const selectedType = typeSelect.value;
        const rules = allRules[selectedType].rules;
        rules.forEach(rule => {
            const option = document.createElement('option');
            option.value = rule.slug;
            option.textContent = rule.name;
            if (rule.slug === key) {
                option.selected = true;
            }
            keySelect.appendChild(option);
        });
        ruleDiv.appendChild(keySelect);
        // Add operator select
        operatorSelect.name = 'operator';
        operators.forEach(op => {
            const option = document.createElement('option');
            option.value = op;
            option.textContent = op;
            if (op === operator) {
                option.selected = true;
            }
            operatorSelect.appendChild(option);
        });
        ruleDiv.appendChild(operatorSelect);
        // Add value input
        valueInput.name = 'value';
        valueInput.value = value;
        valueInput.type = allRules[typeSelect.value].rules.find(rule => rule.slug === keySelect.value).type;
        if (valueInput.type == "checkbox") {
            valueInput.checked = (value === 'true' || value === true);
            if (operatorSelect.selectedIndex == 0) {
                operatorSelect.selectedIndex = Array.from(operatorSelect.options).findIndex(option => option.value === 'bool==');
            }
        }
        ruleDiv.appendChild(valueInput);
        // Add delete button
        deleteButton.type = 'button';
        deleteButton.textContent = 'Delete';
        deleteButton.addEventListener('click', () => {
            rulesContainer.removeChild(ruleDiv);
        });
        // add event listener
        keySelect.addEventListener('change', () => {
            valueInput.type = allRules[typeSelect.value].rules.find(rule => rule.slug === keySelect.value).type;
            if (valueInput.type == "checkbox") {
                valueInput.value = "false";
                operatorSelect.selectedIndex = Array.from(operatorSelect.options).findIndex(option => option.value === 'bool==');
            }else{
                valueInput.value = "";
                operatorSelect.selectedIndex = 0;
            }
        });
        ruleDiv.appendChild(deleteButton);

        rulesContainer.appendChild(ruleDiv);
    }
});