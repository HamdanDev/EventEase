# 🎉 Event Ease App

A modern Blazor-based web application designed for managing corporate and social events with a beautiful, responsive interface.

## 📁 Project Structure

```
EventEase/
├── Pages/
│   ├── Events/                    # Event-related components
│   │   ├── EventList.razor        # Main events grid view
│   │   ├── EventList.razor.css    
│   │   ├── EventDetails.razor     # Event detail view
│   │   ├── EventCard.razor        # Reusable event card
│   │   ├── EventCard.razor.css    
│   │   └── EventRegistration.razor # Event registration form
│   │
│   ├── Shared/                    # Reusable components
│   │   ├── Pagination.razor       # Reusable pagination component
│   │   └── Pagination.razor.css   
│   │
│   ├── Error/                     # Error pages
│   │   ├── NotFound.razor         # 404 page (improved design)
│   │   └── NotFound.razor.css     
│   │
│   └── Home.razor                 # Homepage
│
├── Layout/                        # Application layout
│   ├── MainLayout.razor           # Main app layout
│   ├── MainLayout.razor.css       
│   ├── NavMenu.razor              # Navigation menu
│   └── NavMenu.razor.css          
│
├── Services/                      # Business logic
│   └── EventService.cs           # Event data service
│
└── wwwroot/                      # Static files
    ├── css/
    ├── lib/
    └── index.html
```

---

## First Activity

EventEaseApp is a Blazor-based web application designed for managing corporate and social events. It allows users to browse events, view event details, and register for events seamlessly.

---

## Features

### 1. **Event List Page**
- Displays a list of upcoming events.
- Each event is rendered using the reusable `EventCard` component.
- Includes a "Learn More" button for each event, which can be customized using the `RenderFragment` feature.

### 2. **Event Details Page**
- Displays detailed information about a specific event, including:
  - Event name
  - Event date
  - Event location
- Includes a "Register" button that navigates to the registration page for the selected event.
- Dynamically fetches event details using the `EventService`.

### 3. **Event Registration Page**
- Provides a form for users to register for a specific event.
- Accepts user details such as name and email.

### 4. **Reusable EventCard Component**
- A reusable component for displaying event information.
- Supports dynamic data binding for event name, date, location, and ID.
- Includes a `RenderFragment` parameter for adding custom content (e.g., buttons or links) to the card.

### 5. **Centralized Event Service**
- The `EventService` class manages all event-related data.
- Provides methods to:
  - Fetch all events.
  - Fetch event details by ID.
- Uses mock data for demonstration purposes, which can be replaced with real data from an API or database.

---

## Implementation Details

### EventCard Component
- **File**: `Components/EventCard.razor`
- **Purpose**: Displays event details in a card layout.
- **Features**:
  - Dynamic data binding for event name, date, location, and ID.
  - Customizable content using the `RenderFragment` parameter.
- **Example Usage**:
  ```razor
  <EventCard EventName="Corporate Gala" EventDate="2025-12-01" EventLocation="New York City" EventId="1">
      <button class="btn btn-primary">Learn More</button>
  </EventCard>


## Second Activity

## ✅ Completed Tasks

### 🗑️ Removed Clutter
- Deleted Counter.razor and Weather.razor
- Removed unused navigation links
- Cleaned up unused CSS icons
- Removed sample weather data

### 📂 Organized Event Components
- All event-related components in `Pages/Events/`
- Logical grouping for better maintainability
- Preserved all functionality and routing

### 🔄 Created Reusable Components
- Pagination component in `Pages/Shared/`
- Can be reused across the entire application
- Clean separation of concerns

### 🚫 Enhanced Error Handling
- Modern 404 page design with proper styling
- Multiple routes (`/404` and `/not-found`)
- Better user experience with action buttons

### 🔧 Updated Infrastructure
- Updated `_Imports.razor` for new namespaces
- Cleaned navigation menu
- Fixed all component references
- Removed build warnings

## 🚀 Benefits of New Structure

### Maintainability
- ✅ **Logical Organization**: Related components grouped together
- ✅ **Scalability**: Easy to add more event features or error pages
- ✅ **Clean Separation**: Shared components isolated for reusability

### Development Experience
- ✅ **No Build Errors**: All references updated and working
- ✅ **Consistent Routing**: All routes maintained and functional
- ✅ **Better Navigation**: Cleaner navbar with only relevant links

### Future Expansion
- ✅ **Ready for Growth**: Easy to add new event types or features
- ✅ **Reusable Components**: Pagination can be used anywhere
- ✅ **Error Handling**: Professional error pages ready

---

## Third Activity

## 🎯 Advanced Features Implementation

### 📋 Registration Form with Validation
- **Complete form validation** using DataAnnotations
- **Real-time validation feedback** with Bootstrap styling
- **Field validation** for name, email, phone number, and special requests
- **User-friendly error messages** and success notifications
- **Pre-fill functionality** from user session data
- **Registration status management** (registered, confirmed, cancelled, waitlisted)

### 👤 User Session Tracker for State Management
- **UserSessionService** for comprehensive session management
- **Browser localStorage** integration for data persistence
- **Session tracking** including login state, user preferences, and activity
- **Event registration tracking** within user sessions
- **Automatic session updates** and activity monitoring
- **User preferences management** (theme, notifications, language)

### 📊 Attendance Tracker to Monitor Event Participation
- **AttendanceService** for complete event participation tracking
- **Registration management** with detailed participant information
- **Attendance recording** with check-in/check-out functionality
- **Event statistics** including registration counts and attendance rates
- **Registration status tracking** and cancellation management
- **Session-based data storage** using browser localStorage

## 🔧 Technical Implementation

### Services Architecture
```csharp
// UserSessionService - Session Management
- Login/Logout functionality
- User preferences and settings
- Session persistence with localStorage
- Event registration tracking per user

// AttendanceService - Event Participation
- Event registration with validation
- Attendance recording and tracking
- Registration statistics and reporting
- Cancellation and status management
```

### UI Enhancements
- **Enhanced EventRegistration.razor** with comprehensive form validation
- **Updated EventDetails.razor** showing registration status and participant counts
- **NavMenu integration** displaying logged-in user information
- **Responsive design** for mobile and desktop experiences
- **Real-time updates** with session change notifications

### Data Validation Features
- **Required field validation** for essential information
- **Email format validation** with proper error messaging
- **Phone number validation** for contact information
- **Character limits** for text areas and input fields
- **Custom validation messages** for better user experience

## 🎨 User Experience Improvements

### Registration Flow
- ✅ **Smart pre-filling** from existing user sessions
- ✅ **Registration status indicators** showing current state
- ✅ **Success/error notifications** with clear messaging
- ✅ **Cancellation functionality** with confirmation
- ✅ **Update registration** capability for existing registrations

### Session Management
- ✅ **Persistent user sessions** across browser sessions
- ✅ **User greeting** in navigation with logout functionality
- ✅ **Registration history** tracking per user
- ✅ **Automatic session updates** on user activity
- ✅ **Guest and logged-in** user state management

### Event Participation
- ✅ **Registration tracking** for all events
- ✅ **Attendance statistics** for event organizers
- ✅ **Participant management** with detailed records
- ✅ **Check-in/check-out** functionality for events
- ✅ **Registration status** visibility throughout the app