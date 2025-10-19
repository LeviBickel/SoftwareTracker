# Auth0 Role Configuration

## Role IDs

Copy these IDs from your Auth0 dashboard and use them in your Actions:

### Administrators Role
- **Role ID**: `rol_0rCzDGVxh1aD2g1R` (you provided this)
- **Name**: Administrators
- **Description**: Full system access

### Users Role
- **Role ID**: `GET_THIS_FROM_AUTH0_DASHBOARD`
- **Name**: Users
- **Description**: Standard users

## How to Find Role IDs

1. Go to Auth0 Dashboard → User Management → Roles
2. Click on a role
3. Look at the URL: `https://manage.auth0.com/dashboard/.../roles/rol_XXXXX`
4. Copy the `rol_XXXXX` part

## Action Configuration

### Post User Registration Action
Replace `YOUR_USERS_ROLE_ID_HERE` with the actual Users role ID in your Action.

### Login Action (Add Roles to Token)
No role IDs needed - this reads roles automatically from user's assigned roles.

## Testing

1. Create a new test account (register new user)
2. Check that user is automatically assigned "Users" role
3. Verify in Auth0 Dashboard → User Management → Users → [Your User] → Roles
