import { useAuth } from "@/contexts/AuthContext";
import BarChartIcon from "@mui/icons-material/BarChart";
import BusinessIcon from "@mui/icons-material/Business";
import CategoryIcon from "@mui/icons-material/Category";
import DashboardIcon from "@mui/icons-material/Dashboard";
import LogoutIcon from "@mui/icons-material/Logout";
import PeopleIcon from "@mui/icons-material/People";
import PersonIcon from "@mui/icons-material/Person";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import {
  Box,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Tooltip,
  Typography,
} from "@mui/material";
import { NavLink, useNavigate } from "react-router-dom";

export const DRAWER_WIDTH = 240;

const userNavLinks = [
  { label: "Dashboard", to: "/dashboard", icon: <DashboardIcon /> },
  { label: "My Profile", to: "/profile", icon: <PersonIcon /> },
  { label: "Upload Documents", to: "/upload", icon: <UploadFileIcon /> },
];

const adminNavLinks = [
  {
    label: "Admin Dashboard",
    to: "/admin/dashboard",
    icon: <BarChartIcon />,
  },
  { label: "Employees", to: "/admin/employees", icon: <PeopleIcon /> },
  { label: "Taxonomy", to: "/admin/taxonomy", icon: <CategoryIcon /> },
  { label: "Departments", to: "/admin/departments", icon: <BusinessIcon /> },
];

interface SidebarProps {
  mobileOpen: boolean;
  onMobileClose: () => void;
}

function SidebarContent() {
  const { user, role, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate("/login", { replace: true });
  };

  const links = [...userNavLinks, ...(role === "Admin" ? adminNavLinks : [])];

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        height: "100%",
        overflow: "hidden",
      }}
    >
      {/* Logo / App name */}
      <Box sx={{ px: 2, py: 2.5 }}>
        <Typography variant="h6" fontWeight="bold" noWrap>
          SkillExtractor
        </Typography>
      </Box>
      <Divider />

      {/* Navigation links */}
      <List sx={{ flex: 1, overflowY: "auto", pt: 1 }}>
        {links.map(({ label, to, icon }) => (
          <ListItem key={to} disablePadding>
            <ListItemButton
              component={NavLink}
              to={to}
              end={to === "/dashboard"}
              sx={{
                mx: 1,
                borderRadius: 1,
                "&.active": {
                  bgcolor: "primary.main",
                  color: "primary.contrastText",
                  "& .MuiListItemIcon-root": {
                    color: "primary.contrastText",
                  },
                },
              }}
            >
              <ListItemIcon sx={{ minWidth: 36 }}>{icon}</ListItemIcon>
              <ListItemText primary={label} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>

      <Divider />

      {/* User name + logout */}
      <Box
        sx={{
          px: 2,
          py: 1.5,
          display: "flex",
          alignItems: "center",
          gap: 1,
        }}
      >
        <Box sx={{ flex: 1, minWidth: 0 }}>
          <Typography variant="body2" noWrap>
            {user?.email ?? ""}
          </Typography>
        </Box>
        <Tooltip title="Logout">
          <IconButton size="small" onClick={handleLogout} aria-label="logout">
            <LogoutIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      </Box>
    </Box>
  );
}

export function Sidebar({ mobileOpen, onMobileClose }: SidebarProps) {
  const drawerSx = { width: DRAWER_WIDTH, flexShrink: 0 };
  const paperSx = { width: DRAWER_WIDTH, boxSizing: "border-box" };

  return (
    <Box component="nav" sx={drawerSx} aria-label="main navigation">
      {/* Mobile: temporary drawer */}
      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={onMobileClose}
        ModalProps={{ keepMounted: true }}
        sx={{
          display: { xs: "block", md: "none" },
          "& .MuiDrawer-paper": paperSx,
        }}
      >
        <SidebarContent />
      </Drawer>

      {/* Desktop: permanent drawer */}
      <Drawer
        variant="permanent"
        sx={{
          display: { xs: "none", md: "block" },
          "& .MuiDrawer-paper": paperSx,
        }}
        open
      >
        <SidebarContent />
      </Drawer>
    </Box>
  );
}
