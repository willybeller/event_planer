// Event Planner Alpine.js App
function eventApp() {
    return {
        // Configuration API
        API_BASE_URL: 'http://localhost:5021/api',
        
        // État de l'application
        user: null,
        events: [],
        filteredEvents: [],
        currentFilter: 'all',
        searchTerm: '',
        loading: false,
        
        // État des modals
        showLogin: true,
        showRegister: false,
        showCreateModal: false,
        showEditModal: false,
        showDetailsModal: false,
        
        // Formulaires
        loginForm: {
            email: '',
            password: ''
        },
        registerForm: {
            name: '',
            email: '',
            password: ''
        },
        eventForm: {
            id: null,
            title: '',
            description: '',
            date: '',
            time: '',
            isPrivate: false,
            invitedEmails: []
        },
        
        // Email temporaire pour les invitations
        newInviteEmail: '',
        
        // Événement sélectionné pour les détails
        selectedEvent: null,
        
        // Système d'alertes
        alert: {
            show: false,
            type: 'info',
            message: ''
        },

        // Initialisation
        async init() {
            console.log('Initialisation de Event Planner Alpine.js');
            await this.checkAuthStatus();
            if (this.user) {
                await this.loadEvents();
            }
        },

        // === AUTHENTIFICATION ===
        
        async checkAuthStatus() {
            const token = localStorage.getItem('token');
            const userData = localStorage.getItem('userData');
            
            if (token && userData) {
                try {
                    this.user = JSON.parse(userData);
                    this.showLogin = false;
                    this.showRegister = false;
                    console.log('Utilisateur connecté:', this.user);
                } catch (error) {
                    console.error('Erreur parsing userData:', error);
                    this.logout();
                }
            }
        },

        async login() {
            if (!this.loginForm.email || !this.loginForm.password) {
                this.showAlert('Veuillez remplir tous les champs', 'error');
                return;
            }

            this.loading = true;
            
            try {
                const response = await fetch(`${this.API_BASE_URL}/auth/login`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        Email: this.loginForm.email,
                        Password: this.loginForm.password
                    })
                });

                if (response.ok) {
                    const result = await response.json();
                    
                    // Stocker les données
                    localStorage.setItem('token', result.token);
                    localStorage.setItem('userData', JSON.stringify(result.user));
                    
                    this.user = result.user;
                    this.showLogin = false;
                    this.resetForms();
                    
                    this.showAlert('Connexion réussie !', 'success');
                    await this.loadEvents();
                } else {
                    this.showAlert('Email ou mot de passe incorrect', 'error');
                }
            } catch (error) {
                console.error('Erreur de connexion:', error);
                this.showAlert('Erreur de connexion au serveur', 'error');
            } finally {
                this.loading = false;
            }
        },

        async register() {
            if (!this.registerForm.name || !this.registerForm.email || !this.registerForm.password) {
                this.showAlert('Veuillez remplir tous les champs', 'error');
                return;
            }

            // Validation côté client
            if (this.registerForm.password.length < 6) {
                this.showAlert('Le mot de passe doit contenir au moins 6 caractères', 'error');
                return;
            }

            this.loading = true;

            try {
                const response = await fetch(`${this.API_BASE_URL}/auth/signup`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        Name: this.registerForm.name,
                        Email: this.registerForm.email,
                        Password: this.registerForm.password
                    })
                });

                if (response.ok) {
                    this.showAlert('Inscription réussie ! Vous pouvez maintenant vous connecter.', 'success');
                    this.showLogin = true;
                    this.showRegister = false;
                    this.loginForm.email = this.registerForm.email;
                    this.resetForms();
                } else {
                    // Traitement amélioré des erreurs
                    let errorMessage = 'Erreur lors de l\'inscription';
                    
                    try {
                        const errorData = await response.json();
                        
                        // Vérifier si c'est une erreur de validation
                        if (errorData.errors) {
                            const validationErrors = [];
                            
                            // Traiter les erreurs de validation spécifiques
                            if (errorData.errors.Password) {
                                validationErrors.push('Le mot de passe doit contenir au moins 6 caractères');
                            }
                            if (errorData.errors.Email) {
                                validationErrors.push('Format d\'email invalide');
                            }
                            if (errorData.errors.Name) {
                                validationErrors.push('Le nom est requis');
                            }
                            
                            if (validationErrors.length > 0) {
                                errorMessage = validationErrors.join('. ');
                            }
                        } else if (errorData.message) {
                            errorMessage = errorData.message;
                        } else if (errorData.title) {
                            errorMessage = errorData.title;
                        }
                    } catch (parseError) {
                        // Si ce n'est pas du JSON, essayer de récupérer le texte
                        try {
                            const errorText = await response.text();
                            if (errorText.includes('Email already exists')) {
                                errorMessage = 'Cette adresse email est déjà utilisée';
                            } else if (errorText) {
                                errorMessage = errorText;
                            }
                        } catch (textError) {
                            console.error('Erreur lors du parsing de l\'erreur:', textError);
                        }
                    }
                    
                    this.showAlert(errorMessage, 'error');
                }
            } catch (error) {
                console.error('Erreur d\'inscription:', error);
                this.showAlert('Erreur de connexion au serveur', 'error');
            } finally {
                this.loading = false;
            }
        },

        logout() {
            localStorage.removeItem('token');
            localStorage.removeItem('userData');
            this.user = null;
            this.events = [];
            this.filteredEvents = [];
            this.showLogin = true;
            this.showRegister = false;
            this.resetForms();
            this.showAlert('Déconnexion réussie', 'info');
        },

        // === GESTION DES ÉVÉNEMENTS ===

        async loadEvents() {
            const token = localStorage.getItem('token');
            if (!token) {
                this.logout();
                return;
            }

            try {
                const response = await fetch(`${this.API_BASE_URL}/events`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (response.ok) {
                    const rawEvents = await response.json();
                    
                    // Normaliser les événements
                    this.events = rawEvents.map(event => this.normalizeEvent(event));
                    this.filterEvents();
                    
                    console.log('Événements bruts reçus de l\'API:', rawEvents);
                    console.log('Événements normalisés:', this.events);
                    
                    // Debug: examiner les propriétés de chaque événement
                    this.events.forEach((event, index) => {
                        console.log(`Événement ${index} "${event.title}":`, event);
                        console.log(`  - Propriétés disponibles:`, Object.keys(event));
                        console.log(`  - isPrivate:`, event.isPrivate);
                        console.log(`  - invitedEmails:`, event.invitedEmails);
                        console.log(`  - participants:`, event.participants);
                        console.log('---');
                    });
                } else if (response.status === 401) {
                    this.logout();
                } else {
                    this.showAlert('Erreur lors du chargement des événements', 'error');
                }
            } catch (error) {
                console.error('Erreur lors du chargement:', error);
                this.showAlert('Erreur de connexion au serveur', 'error');
            }
        },

        filterEvents() {
            let filtered = [...this.events];

            // Filtrer par type
            switch (this.currentFilter) {
                case 'my':
                    filtered = filtered.filter(event => event.creatorId === this.user.id);
                    break;
                case 'joined':
                    filtered = filtered.filter(event => 
                        event.participants && 
                        event.participants.some(p => p.userId === this.user.id)
                    );
                    break;
                case 'private':
                    filtered = filtered.filter(event => event.isPrivate === true);
                    break;
                case 'public':
                    filtered = filtered.filter(event => !event.isPrivate);
                    break;
            }

            // Filtrer par recherche
            if (this.searchTerm) {
                const term = this.searchTerm.toLowerCase();
                filtered = filtered.filter(event =>
                    event.title.toLowerCase().includes(term) ||
                    (event.description && event.description.toLowerCase().includes(term))
                );
            }

            this.filteredEvents = filtered;
        },

        async saveEvent() {
            if (!this.eventForm.title || !this.eventForm.date || !this.eventForm.time) {
                this.showAlert('Veuillez remplir tous les champs obligatoires', 'error');
                return;
            }

            // Validation pour les événements privés
            if (this.eventForm.isPrivate && (!this.eventForm.invitedEmails || this.eventForm.invitedEmails.length === 0)) {
                this.showAlert('Veuillez inviter au moins une personne pour un événement privé', 'error');
                return;
            }

            this.loading = true;
            const token = localStorage.getItem('token');

            try {
                // Essayer les deux conventions de nommage pour compatibilité
                const eventData = {
                    Title: this.eventForm.title,
                    Description: this.eventForm.description,
                    Date: this.eventForm.date,
                    Time: this.eventForm.time,
                    IsPrivate: Boolean(this.eventForm.isPrivate),
                    isPrivate: Boolean(this.eventForm.isPrivate), // camelCase aussi
                    InvitedEmails: this.eventForm.isPrivate ? this.eventForm.invitedEmails : [],
                    invitedEmails: this.eventForm.isPrivate ? this.eventForm.invitedEmails : [] // camelCase aussi
                };

                // Debug: afficher les données envoyées
                console.log('Données envoyées à l\'API:', eventData);
                console.log('IsPrivate (type):', typeof eventData.IsPrivate, eventData.IsPrivate);
                console.log('InvitedEmails:', this.eventForm.invitedEmails);

                const url = this.eventForm.id 
                    ? `${this.API_BASE_URL}/events/${this.eventForm.id}` 
                    : `${this.API_BASE_URL}/events`;
                
                const method = this.eventForm.id ? 'PUT' : 'POST';

                const response = await fetch(url, {
                    method: method,
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(eventData)
                });

                if (response.ok) {
                    const result = await response.json();
                    console.log('Réponse de l\'API après création/modification:', result);
                    
                    this.closeModals();
                    const successMessage = this.eventForm.id 
                        ? 'Événement modifié avec succès !' 
                        : this.eventForm.isPrivate 
                            ? `Événement privé créé et ${this.eventForm.invitedEmails.length} invitation(s) envoyée(s) !`
                            : 'Événement créé avec succès !';
                    
                    this.showAlert(successMessage, 'success');
                    
                    // Délai avant rechargement pour permettre de voir les logs
                    console.log('Attente de 3 secondes avant rechargement pour debug...');
                    setTimeout(async () => {
                        await this.loadEvents();
                    }, 3000);
                } else if (response.status === 401) {
                    this.logout();
                } else {
                    const error = await response.text();
                    this.showAlert('Erreur lors de la sauvegarde: ' + error, 'error');
                }
            } catch (error) {
                console.error('Erreur lors de la sauvegarde:', error);
                this.showAlert('Erreur de connexion au serveur', 'error');
            } finally {
                this.loading = false;
            }
        },

        editEvent(event) {
            this.eventForm = {
                id: event.id,
                title: event.title,
                description: event.description || '',
                date: event.date,
                time: event.time,
                isPrivate: event.isPrivate || false,
                invitedEmails: event.invitedEmails ? [...event.invitedEmails] : []
            };
            this.showEditModal = true;
        },

        async deleteEvent(eventId) {
            if (!confirm('Êtes-vous sûr de vouloir supprimer cet événement ?')) {
                return;
            }

            const token = localStorage.getItem('token');

            try {
                const response = await fetch(`${this.API_BASE_URL}/events/${eventId}`, {
                    method: 'DELETE',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (response.ok) {
                    this.showAlert('Événement supprimé avec succès !', 'success');
                    await this.loadEvents();
                } else if (response.status === 401) {
                    this.logout();
                } else {
                    this.showAlert('Erreur lors de la suppression', 'error');
                }
            } catch (error) {
                console.error('Erreur lors de la suppression:', error);
                this.showAlert('Erreur de connexion au serveur', 'error');
            }
        },

        async joinEvent(eventId) {
            const token = localStorage.getItem('token');

            try {
                const response = await fetch(`${this.API_BASE_URL}/events/${eventId}/join`, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (response.ok) {
                    this.showAlert('Vous avez rejoint l\'événement avec succès !', 'success');
                    await this.loadEvents();
                } else if (response.status === 401) {
                    this.logout();
                } else {
                    const error = await response.text();
                    this.showAlert('Erreur lors de la participation: ' + error, 'error');
                }
            } catch (error) {
                console.error('Erreur lors de la participation:', error);
                this.showAlert('Erreur de connexion au serveur', 'error');
            }
        },

        async showEventDetails(event) {
            // Charger les détails complets de l'événement
            const token = localStorage.getItem('token');

            try {
                const response = await fetch(`${this.API_BASE_URL}/events/${event.id}`, {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                });

                if (response.ok) {
                    this.selectedEvent = await response.json();
                    this.showDetailsModal = true;
                } else {
                    this.selectedEvent = event; // Fallback aux données locales
                    this.showDetailsModal = true;
                }
            } catch (error) {
                console.error('Erreur lors du chargement des détails:', error);
                this.selectedEvent = event; // Fallback aux données locales
                this.showDetailsModal = true;
            }
        },

        // === FONCTIONS UTILITAIRES ===

        normalizeEvent(event) {
            // Normaliser les noms de propriétés entre PascalCase et camelCase
            return {
                ...event,
                isPrivate: event.isPrivate ?? event.IsPrivate ?? false,
                invitedEmails: event.invitedEmails ?? event.InvitedEmails ?? [],
                participants: event.participants ?? event.Participants ?? []
            };
        },

        formatDate(dateString) {
            if (!dateString) return '';
            
            try {
                const date = new Date(dateString);
                return date.toLocaleDateString('fr-FR', {
                    weekday: 'long',
                    year: 'numeric',
                    month: 'long',
                    day: 'numeric'
                });
            } catch (error) {
                return dateString;
            }
        },

        isPastEvent(event) {
            if (!event || !event.date || !event.time) return false;
            
            try {
                const eventDateTime = new Date(`${event.date}T${event.time}`);
                return eventDateTime < new Date();
            } catch (error) {
                return false;
            }
        },

        isParticipant(event) {
            if (!event || !event.participants || !this.user) return false;
            return event.participants.some(p => p.userId === this.user.id);
        },

        canJoinEvent(event) {
            if (!event || !this.user) return false;
            
            // Si l'événement est public, tout le monde peut rejoindre
            if (!event.isPrivate) return true;
            
            // Si l'événement est privé, vérifier si l'utilisateur est invité
            const invitedEmails = event.invitedEmails || [];
            return invitedEmails.includes(this.user.email);
        },

        showAlert(message, type = 'info') {
            this.alert = {
                show: true,
                type: type,
                message: message
            };

            // Auto-hide après 5 secondes
            setTimeout(() => {
                this.hideAlert();
            }, 5000);
        },

        hideAlert() {
            this.alert.show = false;
        },

        closeModals() {
            this.showCreateModal = false;
            this.showEditModal = false;
            this.showDetailsModal = false;
            this.resetEventForm();
        },

        resetForms() {
            this.loginForm = { email: '', password: '' };
            this.registerForm = { name: '', email: '', password: '' };
            this.resetEventForm();
        },

        resetEventForm() {
            this.eventForm = {
                id: null,
                title: '',
                description: '',
                date: '',
                time: '',
                isPrivate: false,
                invitedEmails: []
            };
            this.newInviteEmail = '';
        },

        // === GESTION DES INVITATIONS ===

        addInvite() {
            if (!this.newInviteEmail) {
                this.showAlert('Veuillez saisir un email', 'error');
                return;
            }

            // Validation email
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(this.newInviteEmail)) {
                this.showAlert('Format d\'email invalide', 'error');
                return;
            }

            // Vérifier que l'email n'est pas déjà dans la liste
            if (this.eventForm.invitedEmails.includes(this.newInviteEmail)) {
                this.showAlert('Cette personne est déjà invitée', 'error');
                return;
            }

            // Vérifier que ce n'est pas l'email de l'utilisateur connecté
            if (this.newInviteEmail === this.user.email) {
                this.showAlert('Vous ne pouvez pas vous inviter vous-même', 'error');
                return;
            }

            // Ajouter l'email à la liste
            this.eventForm.invitedEmails.push(this.newInviteEmail);
            this.newInviteEmail = '';
            
            this.showAlert('Invitation ajoutée', 'success');
        },

        removeInvite(index) {
            this.eventForm.invitedEmails.splice(index, 1);
            this.showAlert('Invitation supprimée', 'info');
        }
    }
}